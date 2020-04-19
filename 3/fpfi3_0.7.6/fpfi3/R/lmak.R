#' @title Stem bucking simulation and merchantable volume calculation
#' @md
#' @description Lmak simulates a stem bucking process based on a list of products of fixed(same) length.
#'
#' @param treesDiam A data.table or matrix, where every row corresponds to a tree and every column to a diameter at a given bin.
#' @param treesDat A data.table dataframe, with colnames idseq, ht and dbh at least. Every row corresponds to a tree (in the same order as treeVol).
#' @param products  A data.frame of 6 columns and n rows, with product class information. The order is important,
#' it will mark the priority (first products go before the others in Lmak):
#' * Column 1 is the small-end diameter ("diameter").
#' * Column 2 is product length ("length") which has to be the same for all poducts.
#' * Column 3 is the product value ("value").
#' * Column 4 is the product name ("name").
#' * Column 5 is the log type ("log_type"), that indicate if the log is prunned ("p") or non-prunned ("u").
#' * Column 6 is the log class ("class"), that is used to aggregate or classify the data.
#' @param hp Prunning height. Could be NULL (no prunning), a scalar indicating a common hp or
#' a character indicating the column in data that corresponds to every tree hp. Default is NULL.
#' @param lseg0 Segment size (in m) for the volume estimation using the trapezoid aproximation. Default is 0.2 (NOT IMPLEMENTED).
#' @param volform Volume formula to use. Currently log volumes are calculated using the Smalian formula ("smalian"),
#' Newton formula ("newton"), geometric formula ("geometric"), Hoppus formula ("hoppus"), and JAS formula ("jas").
#' See \code{\link{volform_selec}} for more details. Default is "smalian".
#' @param grade_wd If TRUE, logs are graded after applying the discount to the middle girth. Default is TRUE.
#' @param mg_disc Discount in cm to be applied to the middle girth of the logs. Default is NULL (no discount).
#' @param length_disc Discount in cm to be applied to the merchantable lengths of the logs. Default is NULL (no discount).
#'
#' @details This functions is designed to be used by \code{\link[fpfi3]{DoBucking}}. Be carefull to give all the arguments in the proper as no major
#' checks will be performed (all checks and data preparation are made by the \code{\link[fpfi3]{DoBucking}} function).
#' @return A data.table dataframe.
#' @rdname lmak
#'
#' @author Christian Mora, \email{crmora@fibraconsult.cl}, Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @export
#' @import data.table
#' @examples 1+1
#'
lmak <- function (treesDiam, treesDat, products, hp = NULL, lseg0 = 0.2,
                  volform = "smalian",
                  grade_wd = FALSE, mg_disc = NULL, length_disc = NULL)
{
  requireNamespace("data.table")

  # To avoid RMD checks notes:
  idseq <- NULL; volume <- NULL; grade <- NULL; log_type <- NULL; pruned_logs <- NULL; product <- NULL; diameter <- NULL

  void0 <- treesDat
  cut_diam <- treesDiam
  l_length <- max(unique(products[, c("length")]))
  volform_selected <- volform

  # 0.- Internal functions
  # isempty <- function(x) {
  #   if (is.vector(x))
  #     return(length(x) == 0)
  #   else return(any(dim(x) == 0))
  # }

  gradeFunc <- function(x, products) { # changed!
    # diam <- products[order(products[, "diameter"], decreasing=T), ][, "diameter"]
    # x <- cut_diam_wd

    # To avoid RMD checks notes:
    log_type <- NULL

    if (any(x < 0)){
      stop("gradFunc in lmak (DoBucking): all inputs diameters must be equals or greater than zero.")
    }

    products[, log_type:=tolower(log_type)]
    diam <- unlist(products[log_type != 'p', "diameter"])
    pnames <- unlist(products[log_type != 'p', "name"])
    if (diam[length(diam)] != 0) {
      diam <- c(diam, 0)
      pnames <- c(pnames, "discard")
    }

    grad <- matrix(pnames[1], ncol=ncol(x), nrow=nrow(x))
    for (p in 1:(length(diam) - 1)) {
      grad[x >= diam[p + 1] & x < diam[p]] <- pnames[p + 1]
    }
    return(data.table(grad))
  }

  # Descuentos comerciales son aplicados antes en lmak que en lp
  cut_diam_wd <- cut_diam - ifelse(length(mg_disc) == 0, 0, mg_disc)/pi
  cut_diam_wd[cut_diam_wd < 0] <- 0
  l_length_wd <- l_length - ifelse(length(length_disc) == 0, 0, length_disc)/100

  # 6.- Volume
  # system.time(log_vol <- foreach(i = 1:nrow(cut_diam_wd), .combine = rbind) %dopar% volFunc(as.data.frame(cut_diam_wd[i, ])))
  log_vol <- volform_selec(volform_selected) (cut_diam_wd, l_length_wd=l_length_wd)

  if (volform_selected == "smalian") cut_diam_wd <- cut_diam_wd[, -1]
  if (volform_selected == "newton") cut_diam_wd <- cut_diam_wd[, seq(3, ncol(cut_diam_wd), by = 2),  with = F]

  # 7.- Grading
  log_grade <- gradeFunc(cut_diam_wd, products=products)  # 0.15 s

  # 8.- Checking output of Taper, Volume and Grading
  if (!(nrow(cut_diam_wd) == nrow(log_vol) & nrow(log_vol) == nrow(log_grade))) {
    stop("Something go wrong with the dimensions")
  }

  # 9.- Reconciling ids and names
  colnames(cut_diam_wd) <- colnames(log_vol) <- colnames(log_grade) <- as.character(1:ncol(cut_diam_wd))
  cut_diam_wd[, idseq:=1:.N]
  log_vol[, idseq:=1:.N]
  log_grade[, idseq:=1:.N]
  # cut_diam_wd <- cbind(idseq, cut_diam_wd)
  # log_vol <- cbind(idseq, log_vol)
  # log_grade <- cbind(idseq, log_grade)
  # colnames(cut_diam_wd) <- colnames(log_vol) <- colnames(log_grade) <- c(paste("log_", seq(1:n_columns), sep = ""))

  # 10.- Melting data
  tree_result <- melt(data.table(cut_diam_wd), "idseq", variable.name="log", value.name="diameter")
  tree_result[, log:=as.numeric(log)]
  tree_result[, volume:=melt(data.table(log_vol), "idseq", variable.name="log", value.name="vol")[, "vol"]]
  tree_result[, grade:=melt(data.table(log_grade), "idseq", variable.name="log", value.name="vol")[, "vol"]]
  tree_result[, log_type:="u"]
  tree_result <- tree_result[grade != "discard", ]
  if (!is.null(hp)) {
    if (is.numeric(hp)) {
      tree_result[, hp:=hp]
    } else {
      tree_result <- merge(tree_result, void0[, c("idseq", hp), with=F])
    }
    tree_result[, pruned_logs:=trunc(hp/l_length_wd)]
    tree_result[log <= pruned_logs, log_type:="p"]
    tree_result[, pruned_logs:=NULL]
  }
  tree_result[, product:=paste0(grade, ".", log_type)]

  # 11.- Adding missing products. Is this step necessary?
  names_all <- paste(products$name, tolower(products$log_type), sep=".")
  missing_names <- names_all[names_all %in% unique(tree_result[["product"]]) == FALSE]
  if (length(missing_names) > 0) {
    lastlog <- max(tree_result$idseq) + 1
    missing_add <- data.frame(idseq = rep(lastlog, length(missing_names)), log = 9999, diameter = 0,
                              volume = 0, grade = "x", log_type = 0, product = missing_names)
    tree_result <- rbindlist(list(tree_result, missing_add), fill=T)
  }
  tree_result <- tree_result[order(tree_result[, "idseq"]), ]
  tree_result <- tree_result[diameter > 0, ]

  voidp <- merge(x = tree_result[, !"hp"],
                 y = products[, c("name", "log_type", "value", "class"), with=F],
                 by.x=c("grade", "log_type"), by.y=c("name", "log_type"), all.x = TRUE)
  void1 <- merge(x = voidp, y = void0, by = "idseq", all.x = TRUE)
  # 12.- Summarizing
  # tree_summ <- dcast(tree_result, idseq~cat, fun.aggregate=sum, na.rm=T, value.var="volume")
  # # tree_summ[, c("discard_u", "discard_p"):=NULL]
  # setcolorder(tree_summ, c("idseq", sort(colnames(tree_summ)[-1])))
  # tree_summ[, merchvol:=rowSums(.SD), .SDcols=colnames(tree_summ)[-1]]
  # tree_summ <- tree_summ[rownames(tree_summ) != paste(lastlog), ]
  #
  # # 13.- Preparing output
  # if (output_format == "tree") {
  #   void1 <- merge(void0, tree_summ, by="idseq")
  #   void1[, idseq:=NULL]
  # } else if (output_format == "logs") {
  #   tree_result <- tree_result[order(tree_result[, "idseq"]), ]
  #   tree_result <- tree_result[diameter > 0, ]
  #   # row.names(tree_result) <- NULL
  #   # if (is.null(hp)) {
  #   #   void1 <- merge(x = tree_result, y = as.data.frame(void0[,
  #   #     c("treeid", "dbh", "ht", "idseq")]), by.x = "idseq",
  #   #     by.y = "idseq", all.x = TRUE)
  #   # }
  #   # if (!is.null(hp)) {
  #   #   void1 <- merge(x = tree_result, y = as.data.frame(void0[,
  #   #     c("treeid", "dbh", "ht", "hp", "idseq")]),
  #   #     by.x = "idseq", by.y = "idseq", all.x = TRUE)
  #   # }
  #   void1 <- merge(x = tree_result, y = void0, by = "idseq", all.x = TRUE)
  #   void1[, idseq:=NULL]
  # }
  return(void1)
}

