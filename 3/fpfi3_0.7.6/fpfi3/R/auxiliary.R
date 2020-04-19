# system("R CMD Rd2pdf .")
#### ------------- Aux ----------------------

BafterThinDSS <- function(G, N, Nafter) { # euca = uruguay
  ((1.1499*G^0.9356)*(1-(1-Nafter/N)^1.5167)^0.9887)
}

BafterThinPRD <- function(G, N, Nafter) { # eucalyptus = pradaria
  ((0.99037*G^0.96030)*(1-(1-Nafter/N)^1.54178)^1.24752)
}

# Oscar Garcia Loblolly pine model basal area after thinning for "typical" thinning from below | south-east-usa
BafterThinOG <- function(H, G, N, Nafter) {
  (G ^ (-0.4874) - 16.46 * H ^ (-0.6541) * (N ^ (-0.3965) - Nafter ^ (-0.3965))) ^ (-1 / 0.4874)
}


# Mean (quadratic) dbh from basal area and trees/ha
Dg <- function(N, BA) { # N=Number of trees, G=BA
  200 * sqrt(BA / (pi * N))
}

# Inverse function BA from Dg and N
BA <- function(N, Dg) {
  ((Dg / 200) ^ 2) * pi * N
}

# (dynamic parameteres only by species and region)
# Volume <- function(Hd, BA) {
#   ifelse(BA <= 0, 0, pmax(0, BA*(0.0426 + 0.3866*Hd)))
# }



#' @rdname EstimateDBHSd
#' @title Estimate the standard deviation of the DBH
#' @description Computes an estimation of the standard deviation of the DBH, based on
#'
#' @param SI A scalar numeric, indicating the Site Index.
#' @param N0 A numeric scalar, indicating the initial number of trees per hectare.
#' @param ages A numeric vector, indicating the ages t-1 and t, at which the sd would be estimated.
#' The initial age (at which \code{sdi} is computed), must be the first age (t-1).
#' @param sdi A numeric scalar, indicating the initial standard deviation (at t-1).
#' @param qmds A numeric vector, indicating the initial quadatric mean diameter at t-1 and t.
#' @inheritParams availableSpecies
#' @inheritParams ProjectDominantHeight
#'
#' @details To be completed.
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A numeric scalar or vector (depends on the input) with the estimated standard deviation.
#' @examples 1+1
#' @export
#'
EstimateDBHSd <- function(species, region, SI, N0, ages, sdi, qmds, Alt) {
  ifunc <- list(
    eucalyptus_grandis = list(
      uruguay_solid = function(SI, N0, ages, sdi, qmds, ...) {
        k <- 0.0111 * SI + 0.000111 * N0 - 0.3177
        k <- ifelse(k < 0, -0.0001, -k)
        a2 <- 14.0748 - 0.00544 * N0
        # a3 <- 0.4167 * SI - 3.2128
        ans <- a2 * (sdi / a2)^(log(1 - exp(k * ages[2])) / log(1 - exp(k * ages[1])))
        # ans <- a2 * (sdi / a2)^(log(1 - exp(-.1042 * ages[1])) / log(1 - exp(-.1042 * ages[2])))

        # check if variation coef. is constante trough the age (we don't want it to grow)
        if (ans/qmds[2] > sdi/qmds[1]) ans <- qmds[2] * (sdi/qmds[1])
        return(ans)
      }
    ),
    pinus_radiata = list(
      new_zealand = function(ages, sdi, Alt, ...) {
        X <- ifelse(Alt > 450, 1, 0)
        ans <- exp(log(ans[1])*exp(-0.0725*(ages[1] - ages[2]) + 0.000458*(ages[1]^2 - ages[2]^2)))*
          exp(((24257.6 + 0.0194*Alt^2 - 33.2487*(Alt - 450)*X)/(10000))*(1 - exp(-0.0725*(ages[1] - ages[2]) +
                                                                                    0.000458*(ages[1]^2 - ages[2]^2))))
        # ans <- rep(NA, length(ages))
        # ans[1] <- sdi
        # for (i in 2:length(ages)) {
        #   ans[i] <- exp(log(ans[i - 1])*exp(-0.0725*(ages[i - 1] - ages[i]) + 0.000458*(ages[i - 1]^2 - ages[i]^2)))*
        #     exp(((24257.6 + 0.0194*Alt^2 - 33.2487*(Alt - 450)*X)/(10000))*(1 - exp(-0.0725*(ages[i - 1] - ages[i]) +
        #                                                               0.000458*(ages[i - 1]^2 - ages[i]^2))))
        # }
        return(ans)
      }
    ),
    pinus_taeda = list(
      uruguay = function(SI, N0, ages, sdi, ...) {

        # ans <- rep(NA, length(ages))
        # ans[1] <- sdi
        # for (i in 2:length(ages)) {
        #   ans[i] <- exp(log(ans[i - 1])*exp(-0.0956*(ages[i - 1] - ages[i]) - 0.00747*(ages[i - 1]^2 - ages[i]^2)))*
        #     exp((1.46876 - 0.0012*SI - 0.00021*N0)*(1 - exp(-0.0956*(ages[i - 1] - ages[i]) - 0.00747*(ages[i - 1]^2 - ages[i]^2))))
        # }
        # return(ans)
      }
    )
  )
  return(Dispatcher(lista=ifunc, callFunction='EstimateDBHSd',
                    mget(names(formals()), sys.frame(sys.nframe()))))  # match.call()
}


#' @rdname SiteIndex
#' @title Calculate the site index of one place
#' @description This function project the dominant height at any desired age, using different parameters and structure
#' depending on the species and region.
#'
#' @inheritParams availableSpecies
#' @inheritParams ProjectDominantHeight
#'
#' @details This function is a parser for the previous function, using defined ages by species and region.
#' For all species is age 10, except for pinus Radiata which is 20.
#'
#' See \code{\link[fpfi3]{ProjectDominantHeight}} for more information.
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A scalar numeric.
#' @examples 1+1
#' @export
#'
SiteIndex <- function(species, region, Hd1, Age1, Alt=100) {
  nAge <- ifelse(species == 'pinus_radiata', 20, 10)
  return(ProjectDominantHeight(species, region, Hd1, Age1, nAge, Alt))
}
