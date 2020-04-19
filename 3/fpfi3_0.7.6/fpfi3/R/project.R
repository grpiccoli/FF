#' @rdname GetBAAfterThinning
#' @md
#' @title Project the Basal Area (BA) after the thinning action.
#' @description This function project the basal area after a thinning, using different parameters and structure
#' depending on the species and region.
#'
#' @param BA A scalar numeric indicating the initial BA (previous to thinning).
#' @param N1 Number of trees per hectarea previous to the thinning.
#' @param N2 Number of trees per hectarea after the thinning.
#' @param Hd A scalar numeric indicating the dominant height.
#' @inheritParams availableSpecies
#'
#' @details Models were extracted from:
#' * Eucalyptus:
#'     * Uruguay: to be completed.
#' * Pinus Radiata:
#'     * New Zealand: to be completed.
#' * Pinus Taeda:
#'     * Uruguay: to be completed.
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A scalar numeric.
#' @examples 1+1
#' @export
GetBAAfterThinning <- function(species, region, BA, N1, N2, Hd) {
  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(BA, N1, N2, ...) {
        return((1.1499*BA^0.9356)*(1 - (1 - N2/N1)^1.5167)^0.9887)
      },
      brazil_ms = function(BA, N1, N2, ...) {
        return((0.99037*BA^0.96030)*(1 - (1 - N2/N1)^1.54178)^1.24752)
      }
    ),
    pinus_radiata = list(
      new_zealand = function(BA, N1, N2, ...) {
        return(((1.1499*BA^0.9356)*(1 - (1 - N2/N1)^1.5167)^0.9887))
      }
    ),
    pinus_taeda = list(
      uruguay = function(BA, N1, N2, ...) {
        return((1.1313*BA^1.0057)*(1 - (1 - N2/N1)^0.7662)^0.8712)
      },
      usa = function(BA, N1, N2, Hd, ...) {
        return((BA^(-0.4874) - 16.46*Hd^(-0.6541)*(N1^(-0.3965) - N2^(-0.3965)))^(-1/0.4874))
      }
    ),
    gmelina = list(
      ecuador = function(BA, N1, N2, ...) {

      }
    )
  )
  return(Dispatcher(lista=ifunc, callFunction='GetBAAfterThinning',
                    mget(names(formals()), sys.frame(sys.nframe()))))  # match.call()
}


#' @rdname ProjectBasalArea
#' @md
#' @title Project the Basal Area (BA)
#' @description This function project the basal area at any desired age, using different parameters and structure
#' depending on the species and region.
#'
#' @param BA1 A scalar numeric indicating the initial BA.
#' @param Age1 A scalar numeric indicating the Age when \code{BA1} is measured.
#' @param SI A scalar numeric indicating the Site Index.
#' @param N0 A scalar numeric indicating the number of trees when \code{BA1} is measured.
#' @inheritParams availableSpecies
#' @inheritParams ProjectDominantHeight
#'
#' @details Models were extracted from:
#' * Eucalyptus:
#'     * Uruguay: to be completed.
#' * Pinus Radiata:
#'     * New Zealand: to be completed.
#' * Pinus Taeda:
#'     * Uruguay: to be completed.
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A scalar numeric.
#' @examples 1+1
#' @export
#'
ProjectBasalArea <- function(species, region, BA1, Age1, Age2, SI, N0, Alt=100) {
  # Extra ... in every functions is to avoid crashes, but any further argument passed in this way will be ignored.
  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(BA1, Age1, Age2, SI, N0, ...) {
        a1 <- 48.2004 + 8.7031
        a2 <- 43.9277 + 3.669
        k2 <- -1.0159+0.3249*log(SI)+0.1105*(N0/1000)
        return(a2*(BA1/a2)^ (log(1-exp(-k2*Age2))/(log(1-exp(-k2*Age1)))))
      }
    ),
    pinus_radiata = list(
      new_zealand = function(BA1, Age1, Age2, Alt, ...) {
        return(exp(log(BA1)*exp(-0.1628*(Age2-Age1) + 0.00261*(Age2^2-Age1^2)) +
                     ((44797-8.0659*Alt+0.0491*Alt^2)/(10000))*(1-exp(-0.1628*(Age2-Age1) + 0.00261*(Age2^2-Age1^2)))))
      }
    ),
    pinus_taeda = list(
      uruguay = function(BA1, Age1, Age2, SI, N0, ...) {
        return((-1.8427+2.4264*SI+0.0340*N0)*(1+(((-1.8427+2.4264*SI+0.0340*N0)/BA1)^-0.4248 -1)*exp(-0.1269*(Age2-Age1)))^-(1/-0.4248))
      }
    )
  )
  return(Dispatcher(lista=ifunc, callFunction='ProjectBasalArea',
                    mget(names(formals()), sys.frame(sys.nframe()))))  # match.call()
}


#' @rdname ProjectDominantHeight
#' @md
#' @title Project the Dominant Height
#' @description This function project the dominant height at any desired age, using different parameters and structure
#' depending on the species and region.
#'
#' @param Hd1 Initial Dominant Height.
#' @param Age1 A scalar numeric indicating the Age at wich \code{Hd1} is measured.
#' @param Age2 A scalar integer at which the projection is desired.
#' @param Alt The altitude in meters. Used by some functions only. Default is 100.
#' @inheritParams availableSpecies
#'
#' @details Models were extracted from:
#' * Eucalyptus:
#'     * Uruguay: DSS grandis. Need documentation
#' * Pinus Radiata:
#'     * New Zealand: Guy L. Pinjuv 2006, Equation 4.32, Table 4.7. This functions uses an altitude parameter.
#' * Pinus Taeda:
#'     * Uruguay: SAG
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A scalar numeric.
#' @examples 1+1
#' @export
#'
ProjectDominantHeight <- function(species, region, Hd1, Age1, Age2, Alt=100) {
  # Extra ... in every functions is to avoid crashes, but any further argument passed in this way will be ignored.
  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(Hd1, Age1, Age2, ...) {
        a1 <- 48.2004 + 8.7031
        k1 <- 0.0795
        return(a1*(Hd1/a1)^(log(1-exp(-k1*Age2))/(log(1-exp(-k1*Age1)))))
      }
    ),
    pinus_radiata = list(
      new_zealand = function(Hd1, Age1, Age2, Alt, ...) {
        return(exp(log(Hd1)*((Age1 + 4.4616)/(Age2 + 4.4616))^0.7613 +
                     ((44521.7 - 7.3498*Alt + 0.0342*Alt^2)/10000)*(1 - ((Age1 + 4.4616)/(Age2 + 4.4616))^0.7613)))
      }
    ),
    pinus_taeda = list(
      uruguay = function(Hd1, Age1, Age2, ...) {
        return(28.281*(Hd1/28.281)^(log(1-exp(-0.1047*Age2))/(log(1-exp(-0.1047*Age1)))))
      }
    )
  )
  return(Dispatcher(lista=ifunc, callFunction='ProjectDominantHeight',
                    mget(names(formals()), sys.frame(sys.nframe()))))  # match.call()
}


#' @rdname ProjectNTrees
#' @md
#' @title Project the number of trees.
#' @description This function project the number of trees at any desired age, using different parameters and structure
#' depending on the species and region.
#'
#' @param N0 A scalar indicating the initial number of trees.
#' @param Age1 A scalar numeric indicating the Age at wich \code{N0} is measured.
#' @inheritParams availableSpecies
#' @inheritParams ProjectBasalArea
#'
#' @details Models were extracted from:
#' * Eucalyptus:
#'     * Uruguay: document
#' * Pinus Radiata:
#'     * New Zealand: document
#' * Pinus Taeda:
#'     * Uruguay: document
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A scalar numeric.
#' @examples 1+1
#' @export
#'
ProjectNTrees <- function(species, region, N0, BA1, Age1, Age2, SI) {
  # Extra ... in every functions is to avoid crashes, but any further argument passed in this way will be ignored.
  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(N0, ...) {
        return(N0-N0*.01)
      }
    ),
    pinus_radiata = list(
      new_zealand = function(N0, Age1, Age2, ...) {
        return((N0^-1.10169 + (0.00740/100000)*(Age2^2.4358 - Age1^2.4358))^(1/-1.10169))
      }
    ),
    pinus_taeda = list(
      uruguay = function(N0, BA1, Age1, Age2, SI, ...) {
        return((N0*((Age2/Age1)^-0.1429)*exp((-0.029 + 0.00144*SI)*(Age2 - Age1)) ))
      }
    )
  )
  return(Dispatcher(lista=ifunc, callFunction='ProjectNTrees',
                    mget(names(formals()), sys.frame(sys.nframe()))))  # match.call()
}

#' @rdname ProjectVolume
#' @md
#' @title Project the volume.
#' @description This function project the volume at any desired age, using different parameters and structure
#' depending on the species and region.
#'
#' @param BA A scalar numeric indicating the basal area.
#' @param Hd A scalar numeric indicating the dominant height
#' @inheritParams availableSpecies
#'
#' @details Models were extracted from:
#' * Eucalyptus:
#'     * Uruguay: document
#' * Pinus Radiata:
#'     * New Zealand: document
#' * Pinus Taeda:
#'     * Uruguay: document
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A scalar numeric.
#' @examples 1+1
#' @export
#'
ProjectVolume <- function(species, region, BA, Hd) {
  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(BA, Hd, ...) {
        return(ifelse(BA <= 0, 0, pmax(0, BA*(0.0426 + 0.3866*Hd))))
      }
    ),
    pinus_radiata = list(
      new_zealand = function(BA, Hd, ...) {
        # return(ifelse(BA <= 0, 0, pmax(0, BA*(1.082447 + 0.293274*Hd))))  # From Table 7.11,  Thesis by Zhao 1999
        return(ifelse(BA <= 0, 0, pmax(0, 0.329821*BA*Hd)))  # Panco
      },
      chile = function(BA, Hd, ...) {
        return(ifelse(BA <= 0, 0, pmax(0, BA*(-0.4811 + 0.3489*Hd))))  # Informe tecnico 114. INFOR 1988
      }
    ),
    pinus_taeda = list(
      uruguay = function(BA, Hd, ...) {
        return(ifelse(BA <= 0, 0, pmax(0, BA*(-0.333 + 0.4577*Hd))))
      }
    )
  )
  return(Dispatcher(lista=ifunc, callFunction='ProjectVolume',
                    mget(names(formals()), sys.frame(sys.nframe()))))  # match.call()
}
