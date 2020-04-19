#' @rdname DoBucking
#' @title Stem bucking simulation and merchantable volume calculation
#' @description simulates a stem bucking process based on a list of products using lmak or lp.
#'
#' @param data A data.frame, data.table or matrix, with colnames id, ht, and dbh at least. Every row corresponds to a tree.
#' @param coeffs A vector, with the parameters for the taper model. See \code{\link[fpfi3]{taper_selec}} for more information.
#' @param products A data.frame of 3 o 4 columns at least, depending on the \code{buck} option selected. See \code{\link[fpfi3]{lmak}}
#' and \code{\link[fpfi3]{lpBuck}} for more details.
#' @param id The column name in \code{data}, that corresponds to the input id. It will be returned with the ouput table. Default is NULL (will not be used).
#' @param dbh The name of the dbh column in \code{data}. Default is 'dbh'.
#' @param ht The name of the ht column in \code{data}. Default is 'ht'.
#' @param hp Prunning height. Could be NULL (no prunning), a scalar indicating a common hp or
#' a character indicating the column in \code{data} that corresponds to every tree hp. Default is NULL.
#' @param hm Merchandible heigth. Could be NULL, a scalar indicating a common hm for all trees or
#' a character indicating the column name in \code{data} that corresponds to every tree commercial heigth. Default is NULL
#' @param stump Stump height (in m). Default is 0.1.
#' @param lseg0 Segment size (in m) for the volume estimation using the trapezoid aproximation. Default is 0.2 (NOT IMPLEMENTED).
#' @param hdr_range A vector of c(min, max) ht/dbh ratio. This will be used to validate the incoming data. Trees out of this range will be removed.
#' Default is c(0.5, 2.5) (NOT IMPLEMENTED).
#' @param dbh_range A vector of c(min, max) dbh range. This will be used to validate the incoming data. Trees out of this range will be removed.
#' Default is c(10, 60) (NOT IMPLEMENTED).
#' @param ht_range A vector of c(min, max) ht range. This will be used to validate the incoming data. Trees out of this range will be removed.
#' Default is c(10, 60) (NOT IMPLEMENTED).
#' @param model Name of the taper function to use. See details in \code{\link[fpfi3]{taper_selec}} for more information.
#' @param volform Volume formula to use. Currently log volumes are calculated using the Smalian formula ("smalian"),
#' Newton formula ("newton"), geometric formula ("geometric"), Hoppus formula ("hoppus"), and JAS formula ("jas").
#' See \code{\link[fpfi3]{volform_selec}} for more details. Default is "smalian".
#' @param buck A string indicating the type of bucking to be used; options are 'lmak' or 'lpBuck'. Default is 'lmak'.
#' @param grade_wd If TRUE, logs are graded after applying the discount to the middle girth. Default is TRUE.
#' @param mg_disc Discount in cm to be applied to the middle girth of the logs. Default is 0 (no discount).
#' @param length_disc Discount in cm to be applied to the merchantable lengths of the logs. Default is 0 (no discount).
#' @param output Option 'logs' will return all the logs obtained for each tree without summarizing by log class name;
#' "summary" will also return a summary of the total volume for each log assortment at tree level (including logs). Default is "summary".
#' @param byClass If TRUE, and \code{output = 'summary'}, the data will be aggregated using the class (column \code{class} in
#' the \code{products} table), instead of the log name (column \code{name}). Default is FALSE.
#'
#' @details To be completed.
#' @return A list with one or two data.table data.frames. It depends on \code{output}.
#'
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}, Christian Mora, \email{crmora@fibraconsult.cl}
#' @seealso \code{\link[fpfi3]{taper_selec}}, \code{\link[fpfi3]{volform_selec}}, \code{\link[fpfi3]{lmak}}
#' and \code{\link[fpfi3]{lpBuck}}.
#'
#' @export
#' @import data.table
#' @importFrom numbers mGCD
#' @examples
#' set.seed(87)
#'
#' demo_products
#' taper_coeffs
#' trees <- data.frame(id=1:50, dbh=runif(50)*5 + 35, ht=runif(50)*5 + 30, hp=10)
#' ans <- DoBucking(trees, taper_coeffs, demo_products, id="id", dbh='dbh', ht='ht',
#'                  hp='hp', stump=0.15, model="dss", volform="smalian", buck="lmak",
#'                  grade_wd=TRUE, mg_disc = 0, length_disc = 0, output = "summary",
#'                  byClass = FALSE)
#' ans$summary
#' ans$logs
#'
DoBucking <- function(data, coeffs, products, id=NULL, dbh='dbh', ht='ht', hp=NULL, hm=NULL, stump = 0.1, lseg0 = 0.2,
                      hdr_range=c(0.5, 2.5), dbh_range=c(10, 60), ht_range=c(1.3, 60),
                      model = "dss", volform = "smalian", buck = "lmak", grade_wd = TRUE,
                      mg_disc = 0, length_disc = 0, output = "summary", byClass = FALSE) {
  # products, uni y coeffs deben ser cargados antes (lmak_profiling_testing.R tiene todo lo necesario)
  # registros = 10
  # data = uni[1:registros, ]
  # dbh = 'dbh'
  # ht = 'ht'
  # hp = NULL
  # id = 'idgu'
  # hm = NULL
  # model = "dss"
  # coeffs = params[3, ]
  # products = prods
  # volform = "newton" # "jas" # "smalian"
  # grading = "small"
  # stump = 0.15
  # mg_disc = 0
  # length_disc = NULL
  # grade_wd = TRUE
  # output = "tree"
  # buck = "lpBuck"
  # library(matrixStats)
  # library(data.table)
  # library(numbers)
  # hoppus y geometric va con el centro y con descuento

  # To avoid RMD checks notes:
  idseq <- NULL; diameter <- NULL; merchvol <- NULL

  # 1.- Checking input columns
  void0 <- data.table(data)
  void0[, idseq:=1:.N]

  if (any(!c(dbh, ht) %in% colnames(void0))) {
    stop("dbh or ht columns are not present in input data. Please check")
  }

  if (is.null(dim(coeffs))) {
    m_coeffs <- length(coeffs)
  } else {
    m_coeffs <- ncol(coeffs)
  }

  if (!is.null(id)) {
    if (!id %in% colnames(data)) {
      stop('id column not present in input data. Please check')
    }
  }

  if (!is.null(hp)) {
    if (is.character(hp)) {
      if (!hp %in% colnames(data)) {
        stop(paste0('hp column not present in input data. Please check: ', hp))
      }
    }
  }

  if (!is.null(hm)) {
    if (!hm %in% colnames(data)) {
      stop(paste0('hm column not present in input data. Please check: ', hm))
    }
  }

  if (!buck %in% c("lmak", "lpBuck")) {
    stop(paste0("Invalid buck option. ", buck, " is not a valid option"))
  }

  maxht <- max(void0[, ht, with=FALSE], na.rm = TRUE)
  #####  products <- as.data.frame(products)

  # 2.- Generating internal variables
  # products <- products[order(products[, c("priority")]), ] # DEPRECATED: priority should be defined by the user by setting the order of the products.
  min_diam <- min(products[, "diameter"])
  n_prods <- nrow(products)
  # flag0 <- abs(diff(range(products$length))) < .Machine$double.eps^0.5

  # TODO: eliminar match.arg en model_selected y volform_selected
  model_selected <- match.arg(model, c("fang", "bruce", "cao", "cao2", "dss", "fds", "garay", "mb", "mb_chile",
                                       "mm", "parresol", "poly5", "prad", "sispinus", "warner"))

  volform_selected <- match.arg(volform, c("geometric", "hoppus", "smalian", "newton", "jas"))
  output_format <- match.arg(output, c("summary", "logs"))

  # products <- rbind(products, rep(0, ncol(products)))
  # products[max(nrow(products)), "length"] <- 99
  # rownames(products)[max(nrow(products))] <- "discard"

  # 3.- Selecting models and volume computation formula ???
  # if (!flag0) {
  #   stop('No information to be used')
  # }

  # 4.- Pre-calc info
  if (buck == 'lmak') {
    l_length <- max(unique(products[, c("length")]))
    m_length <- l_length/2
    n_columns <- floor(maxht/l_length) * 2
  } else if (buck == 'lpBuck') {
    l_length <- (mGCD(round(products$length*100))/100)
    if (volform_selected %in% c("newton", "hoppus", "geometric")) {
      if (sum((products[ , "length"] / l_length) %% 2) > 0) {
        l_length <- l_length / 2 # si hay numeros impares, no podremos tener la "mitad" del log, por lo que hay que volver a dividir para evitar ese problema
      }
    }
    n_columns <- ceiling(maxht/l_length) * 2
  }

  if (buck == 'lmak') {
    cut_ht <- seq(stump, by=m_length, length.out=n_columns + 1)
  } else if (buck == 'lpBuck') {
    # TODO check new system using cut_ht_i
    if (volform_selected %in% c("hoppus", "geometric")) {
      cut_ht <- seq(stump, maxht, by=l_length)
    } else {
      cut_ht <- seq(stump, maxht, by=l_length)
    }
  }

  # 5. Taper
  cut_diam <- taper_selec(model_selected, x=void0, coeffs=coeffs, cut_ht=cut_ht, dbh=dbh, ht=ht, hm=hm)
  if (sum(cut_diam) == 0) {
    stop(paste0("The selected taper return only 0s. Please check the coefficients provided. ", model_selected, " with coeffs: ", coeffs))
  }

  # 6. Allocating SED, MED, LED
  if (buck == 'lmak') {
    if (volform_selected %in% c("hoppus", "geometric")) {
      cut_ht_i <- as.character(colnames(cut_diam)[seq(2, length(cut_ht), by=2)])
    }
    if (volform_selected == "smalian") {
      cut_ht_i <- as.character(colnames(cut_diam)[seq(1, length(cut_ht), by=2)])
    }
    if (volform_selected == "newton") {
      cut_ht_i <- as.character(colnames(cut_diam))
    }
    if (volform_selected == "jas") {
      cut_ht_i <- as.character(colnames(cut_diam)[seq(1, length(cut_ht) - 1, by=2)])
    }
    LED <- cut_diam[, colnames(cut_diam)[seq(1, length(cut_ht) - 1, by=2)], with=F]
    LED[, idseq:=1:.N]
    MED <- cut_diam[, colnames(cut_diam)[seq(2, length(cut_ht), by=2)], with=F]
    MED[, idseq:=1:.N]
    SED <- cut_diam[, colnames(cut_diam)[seq(3, length(cut_ht), by=2)], with=F]
    SED[, idseq:=1:.N]
  }

  # 7. Bucking!
  if (buck == 'lmak') {
    ans <- lmak(cut_diam[, cut_ht_i, with=F], void0, products, hp=hp, lseg0 = 0.2,
                volform = volform_selected,
                grade_wd = grade_wd, mg_disc = mg_disc, length_disc = length_disc)
  } else if (buck == 'lpBuck') {
    ans <- lpBuck(cut_diam, void0, products, hp=hp, lseg0 = 0.2,
                  volform = volform_selected,
                  grade_wd = grade_wd, mg_disc = mg_disc, length_disc = length_disc)
  }

  # 8.- Adding LED, MED, SED
  if (buck == 'lmak') {
    LED <- melt(LED, id.vars="idseq", variable.name="LED_h", value.name="LED")
    LED[, log:=1:.N, by=idseq]
    MED <- melt(MED, id.vars="idseq", variable.name="MED_h", value.name="MED")
    SED <- melt(SED, id.vars="idseq", variable.name="SED_h", value.name="SED")
    logs <- cbind(LED, MED[, -1], SED[, -1])
    ans <- merge(ans, logs, by.x=c("idseq", "log"), by.y=c("idseq", "log"), all.x=T, all.y=F)
    ans[, diameter:=NULL]
  }

  # 13.- Preparing output
  if (output_format == "summary") {
    # ans[, volpond:=volume*freq]
    if (byClass) {
      tree_summ <- dcast(ans, idseq~class, fun.aggregate=sum, na.rm=T, value.var="volume")
    } else {
      tree_summ <- dcast(ans, idseq~product, fun.aggregate=sum, na.rm=T, value.var="volume")
    }
    setcolorder(tree_summ, c("idseq", sort(colnames(tree_summ)[-1])))
    tree_summ[, merchvol:=rowSums(.SD), .SDcols=colnames(tree_summ)[-1]]
    void1 <- merge(void0, tree_summ, by="idseq")
  } else {
    void1 <- NULL
  }

  return(list(summary=void1, logs=ans))
}
