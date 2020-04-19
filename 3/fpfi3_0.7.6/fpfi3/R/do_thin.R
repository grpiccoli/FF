#' @rdname DoThinning
#' @title Perform a thinning
#' @description Calculate the thinning outputs of a single stand.
#'
#' @param bat A number indicating the starting average basal area (per ha).
#' @param Dgt A number indicating the starting average DBH/Dg.
#' @param Hdt A number indicating the starting Hd.
#' @param ntrees A number indicating the initial number of trees (per ha).
#' @param nout A number indicating the number of trees to be removed (per ha).
#' @param age0 A number indicating the age in years.
#' @param dist A string, indicating the distribution type to be used to simulate the trees. Options are "normal" or "weibull". Default to normal.
#' As a improvement, this must be separate for thinning (as a vector with different distributions for every thinning) and to produce trees.
#' @param sd0 A number indicating the standard deviation. If 0, the 10\% of Dgt will be used. Default to 0.
#' @param thin_type A string, indicating the thinning type. Options are low, high, dist (distribution sim) or byFunction
#' (use the \code{species} and \code{region} to determinate the model to be used). Default to low.
#' @param thin_coefficient A positive number, indicating the Dgt removed proportion over the remaining Dgt.
#' A value of 1 indicates a sistematic thinning following the 'dist' distribution. Values over 1 follow a "high" thinning type and below 1, follow a "low" thinning type.
#' It will be used only if "dist" is selected as the thin_type mode. Usual values range from 0.9 to 1.1 and valid input range from 0.1 to 1.9.
#' See \code{Details} for more information. Default is 1.
#' @param by_class A number, indicating the class interval for the DBH. If 0, the individual density method will be used. Default is 1.
#' @param thin_output A boolean. If TRUE, additional output will be computed (frequency of trees by dbh and ht class that are thinned).
#' @inheritParams availableSpecies
#'
#' @details When \code{thin_coefficient} is out of the range 0.9 to 1.1, it usually produces thinning trees that do not exist.
#' Inside this function, there's an implementation to correct this, using the \code{\link[fpfi3]{ThinningFit}} function,
#' so it's safe to use values out of that ranges.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A list with the BA and Hd, and thinning output if thin_output is TRUE.
#' @importFrom stats sd weighted.mean
#' @export
# @examples 1+1
DoThinning <- function(species, region, bat, Dgt, Hdt, ntrees, nout, age0=0, dist='normal', sd0=0,
                       thin_type='low', thin_coefficient=1, by_class=1, thin_output=FALSE) {
  # bat=grandis$BA[i-1]; Dgt=grandis$Dg[i-1]; Hdt=grandis$Hd[i-1]; ntrees=grandis$N[i-1]; nout=grandis$N[i-1]-grandis$N[i]
  #
  # age0=9; Hdt=31.969697; N0=346.5423; Dgt=29.55975; bat=23.78195; ntrees=346.5423
  # nout=101.5423; dist="normal"; sd0=0; thin_type="low"; thin_coefficient=0.92; by_class=1; thin_output=T
  #
  # To avoid RMD checks notes:
  Bathin <- NULL; Freq <- NULL

  if (!dist %in% c("normal", "weibull")) {
    stop('dist parameter must be normal or weibull')
  }
  if (!thin_type %in% c("low", "high", "dist", "byFunction")) {
    stop('thin_type parameter must be "low", "high", "dist" or "byFunction"')
  }
  if (sd0==0){
    sd0 <- .1 * Dgt
  }

  if (thin_coefficient < 0.1 | thin_coefficient > 1.9) {
    stop(paste0("thin_coefficient ", thin_coefficient, " is not a valid value. Range go from 0.1 to 1.9."))
  }
  # ntrees, by_class, dist, thin_type="low", Dgt=0, Hdt=0, sd0=0, BA=0, age0=0, full=F
  ntrees <- ceiling(ntrees) # todo: REVISAR!
  nout <- ceiling(nout)     # todo: REVISAR!
  Hdnew <- Hdt
  BAthin <- NULL
  sim_t <- SimulateTrees(ntrees, by_class, dist, Dgt, Hdt, sd0, bat, age0, full=T, species=species, region=region)
  trees <- sim_t$trees
  BAtree <- (0.00007854 * trees^2)
  BAtotal <- sum(BAtree)    # total BA (before thinning)

  if (thin_type %in% c("low", "high")) {
    st <- length(BAtree) - nout
    if (thin_type == "low") {
      ids <- (nout+1):length(BAtree)
      idst <- 1:nout
    } else {
      ids <- 1:st
      idst <- (st+1):length(BAtree)
      if (st > 100) {
        Hdnew <- mean(sim_t$hds[ids])
      } else {
        Hdnew <- mean(sim_t$hds)
        # stop("Can't calculate Hd: The remaining number of trees is less than 100 after thinning")
      }
    }
    BAnew <- sum(BAtree[ids])
  } else if (thin_type == "dist") {
    Dg_thinned <- Dgt * thin_coefficient
    # sd0 = 10% del Dg que se ralea por el momento, no hay funcion de estimacion aun.
    thinb <- SimulateTrees(ntrees=nout, by_class=by_class, dist=dist, Dgt=Dg_thinned, Hdt=Hdt,
                           sd0=.1 * Dg_thinned, BA=bat, age0=age0, full=T, species=species,
                           region=region)
    thin <- ThinningFit(sim_t, thinb)
    BAnew <- BAtotal - sum(0.00007854 * thin$trees^2)
  } else if (thin_type == 'byFunction') {
    BAnew <- GetBAAfterThinning(species = species, region = region, BA = bat, N1 = ntrees,
                                N2 = ntrees - nout, Hd = Hdt)
  }

  BAthin <- BAtotal - BAnew
  if (BAthin <= 0) {
    stop(paste0("The thinned BA can't be negative: ", Bathin))  # change for stopifnot function?
  }

  # bb=union(unique(trees), unique(thin$trees)); hist(trees, breaks=bb, col="grey80"); hist(thin$trees, add=T, col=rgb(0, 1, 0, 0.5), xaxt="n", breaks=bb)
  # t1 <- as.matrix(table(trees)); t2 <- as.matrix(table(thin$trees))
  # tremain <- rowSums(cbind(t1, -1*t2[match(rownames(t1), rownames(t2))]), na.rm=T)
  # tremain <- rep(as.numeric(names(tremain)), tremain)
  # hist(tremain, add=T, breaks=bb, col=rgb(1, 0, 0, 0.5))
  #
  # hist(tremain, breaks=bb, col=rgb(0, 0, 1, 0.5), freq=F, ylim=c(0, .3)); hist(thin$trees, add=T, col=rgb(1, 0, 0, 0.5), xaxt="n", breaks=bb, freq=F)


  if (thin_type == 'byFunction') {
    Dg_thinned <- 2 * ((abs(BAthin) * 10000 / (nout * pi)) ^ 0.5) # BA a DBH
    thinb <- SimulateTrees(nout, by_class, dist, Dg_thinned, Hdt, sd0, bat, age0, full=T,
                           species=species, region=region)
    thin <- ThinningFit(sim_t, thinb) # TODO: revisar si realmente hay que corregir esto... si no se corrige se cae, porque se sacan arboles que no existen
  } else if (thin_type %in% c("low", "high")) {
    tdata <- as.data.frame(table(sim_t$trees[idst]))
    tdata$Var1 <- as.numeric(levels(tdata$Var1))[tdata$Var1]
    colnames(tdata)[1] <- "trees"
    tdata$ht <- as.data.frame(table(sim_t$hds[idst]))[,1]
    tdata$ht <- as.numeric(levels(tdata$ht))[tdata$ht]
    thin <- list(trees=trees[idst], hds=sim_t$hds[idst], class_tree=tdata)
    Dg_thinned <- mean(thin$hds)  # TODO: REVISAR, deberia ser mean(thin$trees)????
  }
  sdt <- rbindlist(list(thin$class_tree[, -3] * -1, sim_t$class_tree[, -3]))
  sdt[, trees:=abs(trees)]  # sin negativos en la categoria de arbol, solo en la frecuencia
  sdtd <- sdt[order(trees), list(d=sum(Freq)), by=trees]
  standing_trees <- rep(sdtd$trees, sdtd$d)  # trees that are not thinned
  sdOut <- sd(rep(standing_trees))

  if (thin_output) {
    fout <- list(BAtotal=BAtotal, BAthinned=BAthin, BAremain=BAnew, Hd=Hdnew, sd=sdOut,
                 full_sim=sim_t, thin_sim=thin, Dg_thinned=Dg_thinned, distribution=dist,
                 standing_trees=standing_trees)
  } else {
    fout <- list(BAtotal=BAtotal, BAthinned=BAthin, BAremain=BAnew, Hd=Hdnew, sd=sdOut, distribution=dist)
  }
  return(fout)
}


#' @rdname ThinningFit
#' @title Adjust the thinning output to fit the initial available trees.
#' @description The thinned diameter class will be forced to fit the maximum available trees per diameter class. Some discrepancies could happen
#' when using a distributed thinning (option \code{'dist'}) in the \code{\link[fpfi3]{DoThinning}} function and the thinned trees could result in
#' removed trees that do not exist. This functions aims to relocate the non existing trees into the initial distribution.
#' If thinned average dbh is lower than initial average dbh, it means it's a 'lower' thinning and the non-existing trees will be
#' sequencially moved to the next category, starting from the lower diameters; otherwhise, when
#' the thinned average dbh is greater than the initial average dbh, the starting point will be the higher diameters.
#'
#' @param treeSim an object of the class \code{TreeSim} returned by the \code{\link[fpfi3]{SimulateTrees}} function, representing the initial trees (previous to thinning).
#' @param thinSim an object of the class \code{TreeSim} returned by the \code{\link[fpfi3]{SimulateTrees}} function, representing the thinned trees.
#'
#' @details Note that this function was implemented to be used inside \code{\link[fpfi3]{DoThinning}}.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A list with the adjusted thinning
#' @export
#' @examples
#' # Usinng simulated trees
#' library(fpfi3)
#'
#' thin_coef <- 0.8
#' Dg <- 21.3
#' sd <- 2.75
#' trees <- SimulateTrees(ntrees=432, by_class=1, dist="normal", Dgt=Dg, Hdt=21.8, sd0=sd,
#' BA=15.4, age0=7, full=TRUE, species='eucalyptus_grandis', region='uruguay_solid')
#' thinned_trees <- SimulateTrees(ntrees=182, by_class=1, dist='normal', Dgt=Dg * thin_coef,
#' Hdt=21.8, sd0=.1 * Dg * thin_coef, BA=15.4, age0=7, full=TRUE,
#'  species='eucalyptus_grandis', region='uruguay_solid')
#'
#' ndata <- ThinningFit(trees, thinned_trees)
#'
#' # Visual check of the results
#' plot(trees$class_tree[, c("trees", "Freq")],
#' type="b", pch=16, xlim=range(ndata$class_tree$trees), xaxt="n")
#' axis(1, ndata$class_tree$trees)
#' abline(v=ndata$class_tree$trees, col=rgb(.7, .7, .7, .5))
#' lines(thinned_trees$class_tree[, c("trees", "Freq")], col="darkgreen")
#' lines(ndata$class_tree[, c("trees", "Freq")], col="red")
#' legend('topright', legend=c("Initial trees", "Initial thinning", "Fitted thinning"),
#' col=c('black', 'darkgreen', 'red'), lty=1)
#'
ThinningFit <- function(treeSim, thinSim) {
  # To avoid RMD checks notes:
  trees <- NULL; Freq <- NULL

  if (class(treeSim) != 'TreeSim') {
    stop(paste0("treeSim input must be an object of class TreeSim (returned by SimulateTrees). See ?SimulateTrees for more information."))
  }
  if (class(thinSim) != 'TreeSim') {
    stop(paste0("thinSim input must be an object of class TreeSim (returned by SimulateTrees). See ?SimulateTrees for more information."))
  }

  freq_stand_tree <- treeSim$class_tree
  freq_thinned <- thinSim$class_tree

  ans <- rbindlist(list(freq_thinned[, -3] * -1, freq_stand_tree[, -3]))  # raleos negativos, para que al sumar se encuentre si se sacan + o - arboles. Y sin ht, que agregamos al final
  ans[, trees:=abs(trees)]  # sin negativos en la categoria de arbol, solo en la frecuencia
  dif <- ans[order(trees), list(d=sum(Freq)), by=trees]  # obtener la diferencia entre lo que hay y lo que se ralea
  if (any(dif$d < 0)) {
    new_thin_class <- data.frame(trees=dif$trees, Freq=0)
    cols = c("Freq")
    new_thin_class[which(dif$trees %in% freq_thinned$trees), cols] <- freq_thinned[, cols]

    loc <- dif$d <= 0
    thind <- avai <- dif$d
    avai[loc] <- 0
    thind[!loc] <- 0

    # If thinned mean dbh is lower than initial dbh, it means it's a 'lower' thinning
    if (weighted.mean(freq_thinned$trees, freq_thinned$Freq) <= weighted.mean(freq_stand_tree$trees, freq_stand_tree$Freq)) {
      fillpos <- min(which(c(cumsum(avai), 0) > abs(sum(thind))))
      locator <- 1:(fillpos - 1)
    } else {
      fillpos <- rev(1:length(loc)) [min(which(c(cumsum(rev(avai)), 0) > abs(sum(thind)) ))]
      locator <- (fillpos + 1):length(loc)
    }
    new_thin_class$Freq[locator] <- new_thin_class$Freq[locator] + avai[locator]
    new_thin_class$Freq <- new_thin_class$Freq + thind
    new_thin_class$Freq[fillpos] <- new_thin_class$Freq[fillpos] + sum(freq_thinned$Freq) - sum(new_thin_class$Freq)

    newThinSim <- thinSim
    newThinSim$class_tree <- merge(new_thin_class, freq_stand_tree[, -2])
    newThinSim$trees <- rep(newThinSim$class_tree$trees, newThinSim$class_tree$Freq)
    newThinSim$hds <- rep(newThinSim$class_tree$ht, newThinSim$class_tree$Freq)

    ans <- newThinSim
  } else {
    ans <- thinSim
  }

  class(ans) <- "FittedThin"
  return(ans)
}
