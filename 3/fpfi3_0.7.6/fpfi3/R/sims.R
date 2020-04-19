#' @rdname SimulateTrees
#' @title Simulate trees
#' @description Auxiliary function for DoThinning, to simulate the trees given a certain distribution and other parameters.
#'
#' @param ntrees A number indicating the initial number of trees (per ha)
#' @param by_class A number, indicating the class interval for the DBH. If 0, the individual density method will be used. Default is 1.
#' @param dist A string, indicating the distribution type to be used to simulate the trees. Options are "normal" or "weibull". Default to normal.
#' As a improvement, this must be separate for thinning (as a vector with different distributions for every thinning) and to produce trees.
#' @param Dgt A number indicating the starting average DBH/Dg
#' @param Hdt A number indicating the starting Hd
#' @param sd0 A number indicating the standard deviation. If 0, an attempt to simulate the sd will be conducted using \code{\link[fpfi3]{EstimateDBHSd}}
#' , otherwise the 10\% of Dgt will be used. Default to 0.
#' @param BA A number indicating the Basal Area
#' @param age0 A number indicating the stand age
#' @param full A boolean. If TRUE, the height and other variables will be computed with dbh. Default is False.
#' @param ... Additional arguments passed to \code{\link[fpfi3]{SimulateTreesHeigth}}
#'
#' @details To be completed.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return An object (list) of the class \code{TreeSim} with several attributes
#' @importFrom stats pnorm pweibull qnorm qweibull
#' @export
# @examples 1+1
SimulateTrees <- function(ntrees, by_class, dist, Dgt=0, Hdt=0, sd0=0, BA=0, age0=0, full=FALSE, ...) {
  # ntrees = 346.5423; by_class=1; dist="normal"; Dgt=29.55975; Hdt=31.9697; sd0=Dgt*.1; BA=23.78195; age0=9; full=T
  hds <- NULL
  ans <- NULL
  rntrees <- ceiling(ntrees) # ?como mejorar esto? n hacia arriba
  intervals <- seq(0, 100, by=by_class)
  inames <- intervals[0:(length(intervals)-1)] + by_class/2

  if (F) { # desactivada por mientras, usando el metodo original
    if (dist == "normal") {
      variable <- qnorm(1:rntrees/(rntrees+1), Dgt, sd0)
    } else if (dist == "weibull") {
      l1 <- mweibull(Dgt, sd0)
      variable <- qweibull(1:rntrees/(rntrees+1), l1$shape, l1$scale)
    }
    trees <- cut(variable, intervals, inames) ## classified trees
    trees <- as.numeric(levels(trees))[trees]
    tdata <- as.data.frame(table(trees))
    tdata$trees <- as.numeric(levels(tdata$trees))[tdata$trees]
  } else {
    if (dist == "normal") {
      variable <- pnorm(intervals, Dgt, sd0)
    } else if (dist == "weibull") {
      l1 <- mweibull(Dgt, sd0)
      variable <- pweibull(intervals, l1$shape, l1$scale)
    }
    tdata <- round(diff(variable) * ntrees, 0)
    names(tdata) <- inames
    tdata <- tdata[tdata > 0]
    trees <- as.numeric(rep(names(tdata), tdata))
    tdata <- data.frame(trees=as.numeric(names(tdata)), Freq=as.numeric(tdata))
  }

  sim_hds <- SimulateTreesHeigth(trees=trees, Hd=Hdt, age=age0, BA=BA, ...)

  if (full) {
    tdata$ht <- as.data.frame(table(sim_hds$Ht))[, 1]
    tdata$ht <- as.numeric(levels(tdata$ht))[tdata$ht]
    ans <- list(trees=trees, hds=sim_hds$Ht, class_tree=tdata,
                d100=sim_hds$d100, tph=sim_hds$tph, alpha=sim_hds$alpha, beta=sim_hds$beta,
                cl=sim_hds$cl)
  } else {
    ans <- list(trees=trees, hds=sim_hds$Ht, class_tree=tdata)
  }

  class(ans) <- 'TreeSim'
  return(ans)
}


#' @rdname SimulateTreesHeigth
#' @title Simulate tree height.
#' @description Function to simulate the individual trees height, from a list of individual dbhs
#'
#' @param trees A numeric vector indicating the dbh of individual trees.
#' @param Hd Dominant height
#' @param age Age (in years)
#' @param BA Basal Area.
#' @param SI Site Index.
#' @param Alt Altitude over the sea level in meters. Default is 100.
#' @inheritParams availableSpecies
#'
#' @details Note that use of certain parameters depends on the species and region.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A list with Ht(trees), d100, tph, alpha and beta
#' @export
#' @examples 1+1
SimulateTreesHeigth <- function(species, region, trees, Hd, age, BA, SI, Alt=100) {
  tph <- length(trees)
  if (length(trees) > 99) {
    D100 <- mean(trees[(length(trees) - 99):length(trees)])
  } else {
    D100 <- mean(trees)
  }

  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(trees, Hd, age, BA, tph, D100, ...) {
        # ex guanare
        b0 <- 5.004816
        b1 <- 1.449269
        b2 <- -0.279918
        c0 <- 5.664878
        c1 <- 0.425256
        c2 <- -0.371666
        c3 <- -0.045761
        c4 <- 0.600952
        c5 <- 3.135963
        Ht <- 1.3 + (b0 + b1 * Hd + b2 * D100) * (trees/(1 + trees))^(c0 + c1 * age + c2 * log(tph) + c3 * BA + c4 * D100/age + c5 * Hd * sqrt(tph)/1000)
        return(list(Ht=Ht, d100=D100, tph=tph, cl=c(age, tph, BA, D100, Hd)))
      },
      uruguay_solid2 = function(trees, Hd, age, BA, tph, D100, ...) {
        b0 <- 8.110969
        b1 <- 1.518069
        b2 <- -0.422175
        c0 <- 11.370291
        c1 <- 0.172806
        c2 <- -0.954703
        c3 <- -0.007475
        c4 <- 0.445627
        c5 <- 3.645339
        Ht <- 1.3 + (b0 + b1 * Hd + b2 * D100) * (trees/(1 + trees))^(c0 + c1 * age + c2 * log(tph) + c3 * BA + c4 * D100/age + c5 * Hd * sqrt(tph)/1000)
        return(list(Ht=Ht, d100=D100, tph=tph, cl=c(age, tph, BA, D100, Hd)))
      },
      brazil_ms = function(trees, Hd, age, BA, tph, D100, ...) {
        b0 <- 9.481578
        b1 <- 1.526315
        b2 <- -0.605886
        c0 <- 10.230924
        c1 <- 0.002549
        c2 <- -0.531357
        c3 <- -0.028619
        c4 <- 0.007618
        c5 <- 3.057642
        Ht <- 1.3 + (b0 + b1 * Hd + b2 * D100) * (trees/(1 + trees))^(c0 + c1 * age + c2 * log(tph) + c3 * BA + c4 * D100/age + c5 * Hd * sqrt(tph)/1000)
        return(list(Ht=Ht, d100=D100, tph=tph, cl=c(age, tph, BA, D100, Hd)))
      }
    ),
    pinus_radiata = list(
      new_zealand = function(trees, age, SI, Alt, ...) {
        Ht <- 1.4 + (0.695955 + 0.666983*age^-0.5 -0.106771*log(SI) + (0.954201 + 0.000741*Alt)/trees)^-5
        return(list(Ht=Ht, d100=NULL, tph=NULL, cl=c(age, SI, Alt)))
      }
    ),
    pinus_taeda = list(
      uruguay = function(trees, Hd, age, BA, tph, ...) {
        return(1.3 + (0.0707 + 1.0638*Hd)*(trees/(1 + trees))^(3.2766 + 0.1021*age - 0.0419*log(tph) - 0.0117*BA + 0.8837*(Hd/age) ))
      },
      usa = function(trees, Hd, age, BA, tph, D100, ...) {

      }
    ),
    gmelina = list(
      ecuador = function(trees, Hd, age, BA, tph, D100, ...) {
        b0 <- 0.413536
        b1 <- 1.495365
        b2 <- -0.201823
        c0 <- 7.131181
        c1 <- 0.463979
        c2 <- -0.647227
        c3 <- -0.066764
        c4 <- 0.004319
        c5 <- 9.761559
        Ht <- 1.3 + (b0 + b1 * Hd + b2 * D100) * (trees/(1 + trees))^(c0 + c1 * age + c2 * log(tph) + c3 * BA + c4 * D100/age + c5 * Hd * sqrt(tph)/1000)
        return(list(Ht=Ht, d100=D100, tph=tph, cl=c(age, tph, BA, D100, Hd)))
      }
    )
  )
  args <- mget(names(formals()), sys.frame(sys.nframe()))
  args$D100 <- D100
  args$tph <- tph
  args$SI <- SiteIndex(species, region, Hd, age, Alt)

  return(Dispatcher(lista=ifunc, callFunction='SimulateTreesHeigth',
                    args))  # match.call()
}
