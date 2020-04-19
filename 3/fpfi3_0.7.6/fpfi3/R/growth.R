#' @rdname Growing
#' @title Main Growing function
#' @description Growings function: from initial conditions, compute Dg, BA, N, Vt and thinning action until certain time period (year)
#'
#' @param Age0 A number indicating the starting average age.
#' @param Hd0 A number indicaring the starting tree dominant height.
#' @param N0 A number indicating the starting number of trees per ha.
#' @param Dg0 A number indicating the starting average DBH/Dg.
#' @param BA0 A number indicarting the starting average basal area (per ha).
#' @param Vt0 A numner indicating the starting average total volume (per ha).
#' @param species A string indicating the species.
#' @param Dg0_sd A number indicating the actual standard deviation of \code{Dg0}. Default is 0 that's mean that an attempt to predict
#' the standard deviation (depending on the species) will be conducted, using \code{\link[fpfi3]{EstimateDBHSd}}.
#' @param years A number indicating the number of years to simulate. Default to 30.
#' @param region A string indicating the region. Default is \code{NULL}.
#' @param thinningAge A vector containing the thinning ages as numbers (in increasing order). Default to no thinning.
#' @param n.afterThin A vector containing the number of trees after the thinning.
#' (related to thinningAge; must have the same length). Default to no thinning.
#' @param thin_type See \code{\link[fpfi3]{DoThinning}} for more details.
#' @param thin_coefficient See \code{\link[fpfi3]{DoThinning}} for more details.
#' @param ... Additional arguments passed to \code{\link[fpfi3]{DoThinning}} function.
#'
#' @details To be completed.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @seealso \code{\link[fpfi3]{EstimateDBHSd}} and \code{\link[fpfi3]{DoThinning}}.
#' @return A data.frame with the Growing
#' @examples 1+1
#' @export
# for (f in list.files("F:/R/Growth/fpfi/R", full.names=TRUE)) parse(f)
Growing <- function(Age0, Hd0, N0, Dg0, BA0, Vt0, species, Dg0_sd=0, years=30, region=NULL,
                   thinningAge=NULL, n.afterThin=NULL, thin_type=NULL, thin_coefficient=NULL, ...){
  # Age0 <- 8.46; Hd0 <- 30.81; N0 <- 350; Dg0 <- 28.32; BA0 <- 22.04; Vt0 <- 259; sd0 <- 3.5; years <- 20; thinningAge=c(9); n.afterThin=c(245)
  # Age0 = 7.571743; Hd0 = 27.388918; N0 = 378; Dg0 = 25.29584; BA0 = 18.730381; Vt0 = 220; years = 25; thinningAge = c(8, 10); n.afterThin = c(320.3, 250); dist = "weibull"; sd0 = 3.5; thin_type = c("low", "PRD"); by_class = 1; thin_coefficient=c(1,1)
  # Age0 = 7.571743; Hd0 = 27.388918; N0 = 378; Dg0 = 25.29584; BA0 = 18.730381; Vt0 = 220; years = 25; thinningAge = NA; n.afterThin = NA; dist = "weibull"; sd0 = 0; thin_type = NA; by_class = 1; thin_coefficient=NA

  if (length(thinningAge) != length(n.afterThin)){
    stop(paste0("thinningAge and n.afterThin must have the same length. If you are using the batch function (using an Excel file or similar as input), ",
                "check that the column corresponding to the thinningAge, have all the multiple values as character separated by commas (,) and are not a number."))
  }

  if (!is.na(thinningAge[1])) {
    if (any(Age0 > thinningAge)) {
      stop(paste0("thinningAge must always be higher than the starting age: ", Age0, " is higher than ", paste(thinningAge, collapse=", ")))
    }
  }

  if (!is.na(n.afterThin[1])) {
    if (any(n.afterThin > N0)) {
      stop(paste0("n.aferThin must always be lower than the starting number of trees (N0): ", N0, " is lower than ", paste(n.afterThin, collapse=", "),
                  ". If it's actually lower, the remaining number of trees at the thinning could still be lower, due to mortality."))
    }
  }

  fg <- availableSpecies(species, region, 'Growing')  # checking species
  sp <- fg$species
  rg <- fg$region

  thinningAge <- as.vector(thinningAge)
  n.afterThin <- as.vector(n.afterThin)
  thin <- ifelse(sum(is.null(thinningAge) | is.na(thinningAge)) > 0, F, T)
  SI <- SiteIndex(sp, rg, Hd0, Age0)  #TODO: make it recalculate after thinning if desired option is mark.

  thin_tabs <- list()
  f_sim <- list()
  grandis <- as.data.frame(matrix(0, ncol=9, nrow=years, dimnames=list(1:years, c("Hd","Age","Dg","BA","N","Vt","sd","thin_trees","thinaction"))))
  grandis[1, ] <- c(Hd0, Age0, Dg0, BA0, N0, Vt0, Dg0_sd, 0, 0)
  grandis$thin_type <- ""
  grandis$thin_coef <- NA
  grandis$distr <- ""
  grandis$Age[2] <- with(grandis, ifelse(ceiling(Age[1]) == Age[1], Age[1] + 1, ceiling(Age[1]))) # if first age is not integer, ceiling; otherwise, age + 1
  grandis$Age[3:nrow(grandis)] <- grandis$Age[2] + 1:(nrow(grandis) - 2) # complete the series from 3rd position to the end

  if (thin) {
    tind <- which(grandis$Age %in% thinningAge) + 1:length(thinningAge)  # positions at where I should thinning
    sumd <- rep(0, nrow(grandis))
    sumd[tind] <- 1
    grandis$Age <- cumsum(sumd) * -1 + grandis$Age
    grandis$thinaction[tind] <- 1  # when to thinning????
    splitvect <- cumsum(grandis$thinaction)
    # splitvect[which(grandis$thinaction == 1)] <- -1
    sd_pos <- split(1:nrow(grandis), splitvect)  # esta lista nos dice donde estan los grupos antes, entre, y despues de raleo (y cuando hay raleo tb)
  } else {
    sd_pos <- list(1:nrow(grandis))
  }

  # Parameters BA projection function
  N1 <- N0
  thincount <- 1

  for (i in 2:length(grandis$Age)) {
    t1 <- grandis$Age[i-1]
    t2 <- grandis$Age[i]
    if (grandis$thinaction[i] == 0) {
      grandis$Hd[i] <- ProjectDominantHeight(sp, rg, grandis$Hd[i-1], t1, t2)
      # grandis$N[i] <- ProjectNTrees(grandis$N[i-1])
      # grandis$BA[i] <- PredB2(grandis$BA[i-1], grandis$Age[i-1], grandis$Age[i], k2)
      grandis$BA[i] <- ProjectBasalArea(sp, rg, grandis$BA[i-1], grandis$Age[i-1], grandis$Age[i], SI, N1, Alt=100)  # replace N1 for N0 to previous results
      grandis$N[i] <- ProjectNTrees(sp, rg, grandis$N[i-1], grandis$BA[i-1], grandis$Age[i-1], grandis$Age[i], SI)
      # grandis$Dg[c(i-1, i)] <- with(grandis, Dg(N[c(i-1, i)], BA[c(i-1, i)]))
      grandis$Dg[i] <- with(grandis, Dg(N[i], BA[i]))
      grandis$sd[i] <- with(grandis, EstimateDBHSd(species=species, region=region, SI=SI, N0=N1, # TODO: es N1 para eucalyptus, uruguay_solid; revisar si es en todos así.)
                                                   ages=Age[c(i-1, i)], qmds=Dg[c(i-1, i)],
                                                   sdi=sd[i - 1], Alt=100))
    } else {
      N1 <- grandis$N[i] <- n.afterThin[thincount]  # Se actualiza N0 (inicial) después del raleo
      # sd_t <- grandis$sd[grandis$sd != 0]
      # sd_ <- EstimateDBHSd(species=sp, region=rg, SI=SI, N0=grandis$N[sd_pos[[thincount]]][1], BA=1,
      #                      ages=grandis$Age[sd_pos[[thincount]]], sdi=grandis$sd[sd_pos[[thincount]]][1], Alt=100)
      # grandis$sd[sd_pos[[thincount]]] <- sd_
      # if (thincount > 2) {
      #   bb = 1 + 1
      # }
      # if (thincount == 1) {
      #   grandis$sd[sd_pos[[thincount + 1]][-1]] <- sd_
      # } else {
      #   grandis$sd[sd_pos[[thincount + 1]]] <- sd_
      # }
      bas <- DoThinning(species=sp, region=rg,
                        bat=grandis$BA[i-1], Dgt=grandis$Dg[i-1], Hdt=grandis$Hd[i-1], ntrees=grandis$N[i-1],
                        nout=grandis$N[i-1]-grandis$N[i], age0=grandis$Age[i], thin_type=thin_type[thincount],
                        thin_coefficient=thin_coefficient[thincount], sd0=grandis$sd[i-1], ...)
      # dist = "weibull", sd0 = 0, by_class = 1, thin_output=T)
      # grandis$Dg <- Dg(grandis$N, grandis$BA)  # I must do this, in order to retrieve the Dg for the thinning process
      f_sim[[thincount]] <- bas
      grandis$Hd[i] <- unlist(bas['Hd'])
      grandis$BA[i] <- unlist(bas['BAremain'])
      grandis$sd[i] <- unlist(bas['sd'])
      grandis$Dg[i] <- with(grandis, Dg(N[i], BA[i]))
      grandis$thin_trees[i] <- with(grandis, N[i-1]-N[i])
      grandis$thin_type[i] <- thin_type[thincount]
      grandis$thin_coef[i] <- thin_coefficient[thincount]
      grandis$distr[i] <- unlist(bas['distribution'])

      thinned <- bas[['thin_sim']]$class_tree
      thinned$n_thin <- thincount
      thin_tabs[[thincount]] <- thinned

      thincount <- thincount + 1
    }
  }

  # grandis$Dg <- Dg(grandis$N, grandis$BA)
  grandis$Vt <- ProjectVolume(sp, rg, grandis$BA, grandis$Hd)
  # grandis$sd[sd_pos[[thincount]]] <- EstimateDBHSd(species=species, region=region, SI=SI, N0=grandis$N[sd_pos[[thincount]]][1],
  #                                                  BA=grandis$N[sd_pos[[thincount]]][1], ages=grandis$Age[sd_pos[[thincount]]],
  #                                                  sdi=grandis$sd[sd_pos[[thincount]]][1], Alt=100)

  #return(list(data=grandis, thin_out=thin_tabs))
  return(list(data=grandis, thin_out=thin_tabs, harvest_detail=f_sim, thin_present=thin))
}


#' @rdname BatchGrowing
#' @title Main Growing batch parser
#' @description Parser to the \code{\link[fpfi3]{Growing}} function, for multiple simulations
#'
#' @param data A matrix with an id, Age, Hd, N, Dg, DBH_sd, BA, Vt, years, thinninAge, n.afterThin, thinTypes and thin_coeff
#' as columns (following the example)
#' @param ... Arguments passed to \code{\link[fpfi3]{Growing}}.
#'
#' @details To be completed.
#'
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @seealso \code{\link[fpfi3]{Growing}}.
#' @return A data.frame with the Growing results
#' @examples
#' values <- c(1, 8.46, 30.813846, 350.0427, 28.08013, 22.043504, 250, 25, '9',
#' '245', "low", "1", 2, 7.571743, 27.388918, 378, 25.29584, 18.730381, 220, 25,
#' '8,10', '320.3,250', "low,PRD", "0.9,0.9", 3, 6.571986, 25.437695, 478.8546,
#' 22.92591, 20.037553, 220, 25, '7,9', '387.1,250', 'low,PRD', "0.9,0.9", 4, 5.52,
#' 22.5, 558.1, 20.6, 18.977, 180, 25,'6,7', '503.8,350', 'low,PRD', "0.9,0.9",
#' 5, 1.621429, 9.792857, 456.7143, 10.20179, 25, 3.852857, 25, '5,7', '400,350',
#' 'low,PRD', "0.9,0.9")
#' @export
BatchGrowing <- function(data, ...) {
  # values <- c(1, 8.46, 30.813846, 350.0427, 28.08013, 22.043504, 250, 25, '9', '245', "low", "1",
  #   2, 7.571743, 27.388918, 378, 25.29584, 18.730381, 220, 25, '8,10', '320.3,250', "low,PRD", "0.9,0.9",
  #   3, 6.571986, 25.437695, 478.8546, 22.92591, 20.037553, 220, 25, '7,9', '387.1,250', 'low,PRD', "0.9,0.9",
  #   4, 5.52, 22.5, 558.1, 20.6, 18.977, 180, 25,'6,7', '503.8,350', 'low,PRD', "0.9,0.9",
  #   5, 1.621429, 9.792857, 456.7143, 10.20179, 25, 3.852857, 25, '5,7', '400,350', 'low,PRD', "0.9,0.9")
  # params <- as.data.frame(matrix(values, ncol=12, byrow=T,
  #                                dimnames=list(NULL, c("id", "Age", "Hd", "N", "Dg", "BA", "Vt", "years", "thinningAge", "n.afterThin", "thinTypes", "thin_coeff"))),
  #                         stringsAsFactors=F)
  # params[, 1:8] <- apply(params[, 1:8], 2, function(x) as.numeric(as.character(x)))
  ans <- list()
  thi <- list()
  det <- list()
  counter <- 1
  counterg <- 1
  paramsg <- data
  for (f in 1:nrow(paramsg)) {
    # print(paste0(f, " -- id: ", paramsg$id[f]))
    anst <- Growing(Age0=paramsg$Age[f], Hd0=paramsg$Hd[f], N0=paramsg$N[f], Dg0=paramsg$Dg[f], BA0=paramsg$BA[f],
                   Vt0=paramsg$Vt[f], Dg0_sd=paramsg$DBH_sd[f], years=paramsg$years[f],
                   thinningAge=as.vector(eval(parse(text=paste0("c(",paramsg$thinningAge[f],")")))),
                   n.afterThin=as.vector(eval(parse(text=paste0("c(",paramsg$n.afterThin[f],")")))),
                   thin_type=unlist(strsplit(paramsg$thinTypes[f], ",")),
                   thin_coefficient=as.vector(eval(parse(text=paste0("c(",paramsg$thin_coeff[f],")")))),
                   ...)
    res <- anst[['data']]
    tfreq <- anst[['thin_out']]

    res$id <- paramsg$id[f]
    res$idg <- counterg:(counterg + nrow(res) - 1)
    ans[[counter]] <- res
    idgs <- res[res$thinaction==1, "idg"]
    # thi[[counter]] <- do.call("rbind", lapply(1:length(idgs), function(x) {y <- tfreq[[x]]; y$idg <- idgs[x]; return(y)}))
    if (anst[['thin_present']]) {
      for (i in 1:length(anst[['harvest_detail']])){
        det[[as.character(idgs[i])]] <- anst[['harvest_detail']][[i]]
      }
    }
    counter <- counter + 1
    counterg <- counterg + nrow(res)
  }

  final <- as.data.frame(do.call("rbind", ans))
  final[, c(1:9)] <- apply(final[, c(1:9)], 2, function(x) as.numeric(as.character(x)))

  # thin_trees <- as.data.frame(do.call("rbind", thi))
  # thin_trees$idm <- 1:nrow(thin_trees)
  return(list(main=final, detail=det))
}


#' @rdname FullSimulation
#' @md
#' @title Run a full Growing simulation.
#' @description Create a full simulation from an initial condition input and returning the full volume simulation (harvest and thinning).
#' The function will start simulating the growth, then the taper and bucking for the harvest (and optionally, the thinning) volume, in order
#' to generate the aggregated yield tables.
#'
#' @param in_data A list with three data.frames, in the form list(df=initial conditions, params=parameteres, prods=products)
#' @param age_range A vector with the c(min, max) age range to be used in the cutting simulation.
#' @param distribution See \code{dist} in \code{\link[fpfi3]{SimulateTrees}}.
#' @param distribution_thinning See \code{dist} in \code{\link[fpfi3]{DoThinning}}.
#' @param include_thinning if TRUE, the yield by production at every thinning will be added to the output. Default is TRUE.
#' @param species A String indicating the species. Options are 'eucalyptus', 'pine', 'teak'.
#' @param model See \code{\link[fpfi3]{taper_selec}}
#' @param volform See \code{\link[fpfi3]{DoBucking}}
#' @param way A string indicating the taper way to compute the volume production. Options are "lmak" or "lpBuck". Defualt is \code{lmak}.
#' See \code{\link{lpBuck}} and \code{\link{lmak}} for more information.
#' @param region An optional string value, indicating the region.
#' If NULL, global parameters for the species will be used. Options are listed in the \code{details} section. Default is NULL.
#' @inheritParams DoBucking
#'
#' @details This function is a wrapper for several individual functions. In first place, an instance of \code{\link[fpfi3]{BatchGrowing}}
#' is used to generate all the simulated properties for every stand, at all the desired ages. That simulation will be used to peform the taper and
#' the bucking/grading by the \code{\link[fpfi3]{DoBucking}}, for harvest and thinning (depending on the condition set in \code{include_thinning}.
#' Ultimately, union/join/merge operations will be used to mantain the consistency between the input tables and the output tables. Additionally
#' information will be added to the output.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @seealso \code{\link[fpfi3]{BatchGrowing}} and \code{\link[fpfi3]{DoBucking}}, for a detailed explanation of the internal methods. To see which
#' species are available \code{\link[fpfi3]{availableSpecies}}.
#'
#' To visualize results, \code{\link[fpfi3]{plotHist}}.
#'
#' @return An S3 object of class \code{FullSimulation}. This object is basically a list, consisting in:
#' * \code{simulation}: A \code{data.table} data.frame with the growth simulation and others computed indices (as columns).
#' * \code{taper}: A list with two levels of details.
#'    * \code{log_level}: A list with the individuals logs by tree, separated by \code{harvest} and \code{thinning}. Each table have:
#'        * \code{harvest}:
#'        * \code{thinning}:
#'    * \code{tree_level}: A list with the individual tree level detail separated by \code{harvest} and \code{thinning}. Each table have:
#'        * \code{harvest}:
#'        * \code{thinning}:
#'    * \code{stand_level}: A list with the summary of \code{tree_level}. It usually used for the LP input and it's separated \code{harvest} and \code{thinning}:
#'        * \code{harvest}:
#'        * \code{thinning}:
#' * \code{input}: A list with the tree inputs:
#'    * \code{initial_conditions}: A \code{data.table} data.frame with the initial condictions for every stand.
#'    * \code{taper_coefficients}: A named vector, with the coefficients for the taper function \code{\link[fpfi3]{taper_selec}}.
#'    * \code{products}: A \code{data.table} data.frame with the initial products.
#' * \code{growth}: A list, returned by \code{\link[fpfi3]{BatchGrowing}}.
#' * \code{cl}: A call returned by \code{match.call}, with the initial parameters of the \code{FullSimulation} call.
#'
#' @export
#' @import data.table
#' @importFrom plyr count
#' @examples
#' library(fpfi3)
#'
#' # Let's use the data loaded inside this package:
#' initial_conditions  # See ?initial_conditions for a detailed description.
#' taper_coeffs  # See ?taper_coeffs for a detailed description.
#' demo_products  # See ?demo_products for a detailed description.
#'
#' # Running the simulation
#' input_data <- list(df=initial_conditions, params=taper_coeffs, prods=demo_products)
#' sims <- FullSimulation(in_data = input_data,
#'                        age_range = c(15,30), distribution = "normal",
#'                        distribution_thinning = "normal", include_thinning = TRUE,
#'                        species="eucalyptus_grandis", model = "dss", volform = "smalian",
#'                        way = "lmak", region = 'uruguay_solid',
#'                        stump=0.15, mg_disc=0, length_disc=0)
#'
FullSimulation <- function(in_data, age_range, distribution, distribution_thinning, species,
                           model, volform, include_thinning=TRUE, byClass=FALSE,
                           way='lmak', region=NULL, stump=0.15, mg_disc=0, length_disc=0) {

  # To avoid RMD checks notes:
  idgu <- NULL; thin_year <- NULL; Age <- NULL; pyear <- NULL; thin.name <- NULL; id <- NULL; Vt <- NULL; thinaction <- NULL
  CAI_Dg <- NULL; CAI_Vt <- NULL; MAI_Dg <- NULL; MAI_Vt <- NULL

  ##---- Internal function -------- #
  PrepStandData <- function(tot, prodn, multi, df) {
    # x <- uni; y <- fr.prod
    # x <- thin; y <- fr.prod.t
    inprod <- prodn[prodn %in% colnames(tot)]
    tot[, (inprod) := lapply(.SD, function(x) x * tot[['freq']]), .SDcols=inprod]
    sumf <- tot[, lapply(.SD, sum, na.rm=TRUE), by=c("idg"), .SDcols=inprod]
    final <- merge(multi$main[, c("id", "idg", "Age")], sumf, by="idg")
    return(merge(df[, c("id", 'macrostand', "pyear")], final, by="id"))
  }
  ##-------------------------------- #

  requireNamespace("data.table")
  fg <- availableSpecies(species, region, 'FullSimulation')  # checking species
  sp <- fg$species
  rg <- fg$region

  df <- data.table(in_data$df)
  params <- unlist(in_data$params)
  prods <- data.table(in_data$prods)
  # rownames(prods) <- prods[, 1]
  # prods <- as.matrix(prods[, -1])

  cat("Using input data to project Growing\n")
  multi <- BatchGrowing(as.data.frame(df), dist=distribution, by_class=1, thin_output=T,
                        region=rg, species=sp)

  #---------------------------------------------------------------------------------------------------------------------------------------
  #---------------------------------------------------------------------------------------------------------------------------------------
  cat("Allocating simulated harvest data for taper\n")
  age_rangev <- seq(age_range[1], age_range[2])
  exput <- multi$main[multi$main$Age %in% age_rangev, ]
  ans <- lapply(1:nrow(exput), function(i) {
    pv <- exput[i, ]
    ans <- with(pv, SimulateTrees(ntrees=N, by_class=1, dist=distribution, Dgt=Dg, Hdt=Hd, sd0=Dg*.1,
                                  BA=BA, age0=Age, full=F, species=sp, region=rg))
    data.table(id=pv$id, idg=pv$idg, dbh=ans$trees, ht=ans$hds)
  })
  allt <- rbindlist(ans)
  uni <- data.table(count(allt))
  uni[, idgu:=1:.N]
  uni <- merge(uni, df[, c("id", "hp", "hm")])

  #---------------------------------------------------------------------------------------------------------------------------------------
  cat("Applying taper over harvested simulated data\n")
  fr.prod <- DoBucking(uni, dbh = 'dbh', ht = 'ht', id='idgu', hp = 'hp', hm = 'hm',
                       model = model, coeffs = params, products = prods, volform = volform,
                       stump = stump, mg_disc = mg_disc, length_disc = length_disc,
                       grade_wd = TRUE, output = "summary", byClass=byClass,
                       buck = way)

  prodn <- colnames(fr.prod$summary)[(which(colnames(fr.prod$summary) %in% "hm")+1):(ncol(fr.prod$summary)-1)]
  # TODO: output va a funcionar bien con lmak, con lpout no lo se
  #---------------------------------------------------------------------------------------------------------------------------------------
  #---------------------------------------------------------------------------------------------------------------------------------------
  if (include_thinning) {
    cat("Allocating simulated thinning data for taper\n")
    ids <- names(multi$detail)
    ansr <- lapply(ids, function(x) {
      cbind(id=multi$main$id[multi$main$idg == x], idg=as.numeric(x), multi$detail[[x]]$thin_sim$class_tree)
    })
    thin <- rbindlist(ansr)
    setnames(thin, c("trees", "Freq"), c("dbh", "freq"))
    thin[, idgu:=1:nrow(thin)]
    thin <- merge(thin, df[, c("id", "hp", "hm")])

    #---------------------------------------------------------------------------------------------------------------------------------------
    cat("Applying taper over thinned simulated data\n")
    fr.prod.t <- DoBucking(thin, dbh = 'dbh', ht = 'ht', id='idgu', hp = 'hp', hm = 'hm',
                           model = model, coeffs = params, products = prods, volform = volform,
                           stump = stump, mg_disc = mg_disc, length_disc = length_disc,
                           grade_wd = TRUE, output = "summary", byClass=byClass,
                           buck = way)
  } else {
    fr.prod.t <- list(summary=NULL, logs=NULL)
  }
  # fr.prod.t$summary[, thinname:=paste0("thinning", 1:.N), by=id]
  #---------------------------------------------------------------------------------------------------------------------------------------
  #---------------------------------------------------------------------------------------------------------------------------------------
  cat("Preparing output\n")

  stand.h <- PrepStandData(fr.prod$summary, prodn, multi, df)
  if (include_thinning) {
    stand.t <- PrepStandData(fr.prod.t$summary, prodn, multi, df)
    stand.t[, thin_year:=Age+pyear]
    stand.t[, thin.name:=paste0("thinning", sprintf("%02d", 1:.N)), by=id]
  } else {
    stand.t <- NULL
  }

  # Graphics data
  g_data <- merge(df[, c("id", "macrostand"), with=F], multi$main)
  g_data[, c("CAI_Dg", "CAI_Vt") := list(Dg - shift(Dg, 1), Vt - shift(Vt, 1)), by=id]
  g_data[thinaction == 1, c("CAI_Dg", "CAI_Vt") := 0]
  g_data[, c("CAI_Dg", "CAI_Vt") := list(thinaction * shift(CAI_Dg, 1) + CAI_Dg, thinaction * shift(CAI_Vt, 1) + CAI_Vt), by=id]
  # g_data[, c("MAI_Dg", "MAI_Vt") := list(cumsum(thinaction * (shift(Dg, 1, fill=0)*shift(N, 1, fill=0) - Dg*N) / (shift(N, 1, fill=0) - N)),
  # g_data[, c("MAI_Dg", "MAI_Vt") := list(Dg - shift(Dg, 1), cumsum(thinaction * (shift(Vt, 1, fill=0) - Vt))), by=id]
  # g_data[, c("MAI_Dg", "MAI_Vt") := list((thinaction * shift(MAI_Dg, 1, fill=0) + MAI_Dg * !thinaction) / Age, (Vt + MAI_Vt)) / Age, by=id]
  # g_data[, c("MAI_Dg", "MAI_Vt") := list((thinaction * shift(MAI_Dg, 1, fill=0) + MAI_Dg * !thinaction) / Age, (Vt + MAI_Vt) / Age), by=id]
  g_data[, c("MAI_Dg", "MAI_Vt") := list(Dg/Age, cumsum(thinaction * (shift(Vt, 1, fill=0) - Vt))), by=id]
  g_data[, c("MAI_Dg", "MAI_Vt") := list(thinaction * shift(MAI_Dg, 1, fill=0) + MAI_Dg * !thinaction, (Vt + MAI_Vt) / Age), by=id]

  ans <- list(simulation=g_data,
              taper=list(
                stand_level=list(
                  harvest=stand.h,
                  thinning=stand.t),
                tree_level=list(
                  harvest=fr.prod$summary,
                  thinning=fr.prod.t$summary),
                log_level=list(
                  harvest=fr.prod$logs,
                  thinning=fr.prod.t$logs)
              ),
              input=list(initial_conditions=df,
                         taper_coefficients=params,
                         products=prods),
              growth=multi,
              cl=match.call()
  )

  class(ans) <- 'FullSimulation'
  return(ans)
}

