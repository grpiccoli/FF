#' @title Volume form selector.
#' @md
#'
#' @description Select the desired function.
#'
#' @param func_name A string indicating the model to be used. See Details for more information.
#'
#' @details There are 5 models available. They use different parts of the log to make the volume calculation, based on the large end diameter (LED),
#' middle diameter (MID) or small end diameter (SED). See the pdf version of this help if the equations are not displayed correctly.
#'
#'
#' \code{geometric}: Usually used in teak. The volume calculations uses the log MID.
#'
#' \deqn{V(m^3) = MID^2 \times L \times 6.1685^{-5}}
#' where:
#' * MID = Middle Diameter in cm.
#' * L = length in m.
#'
#'
#' \code{hoppus}: Usually used in teak. The volume calculations uses the log MID.
#'
#' \deqn{V(m^3) = \frac{MID^2}{40000} \times pi \times L}
#' where:
#' * MID = Middle Diameter in cm.
#' * L = length in m.
#'
#'
#' \code{smalian}: The volume calculations uses the log LED and SED.
#'
#' \deqn{V(m^3) = \frac{LED^2 + SED^2}{80000} \times pi \times L}
#' where:
#' * LED = Large End Diameter in cm.
#' * SED = Small End Diameter in cm.
#' * L = length in m.
#'
#'
#' \code{newton}: The volume calculations uses the log LED, MID and SED.
#'
#' \deqn{V(m^3) = \frac{LED^2 + 4 \times MID^2 + SED^2}{240000} \times pi \times L}
#' where:
#' * LED = Large End Diamater in cm.
#' * MID = Middle Diameter in cm.
#' * SED = Small End Diameter in cm.
#' * L = length in m.
#'
#'
#' \code{jas}: Japanese Agricultural Standard (JAS). The volume calculations uses the log SED.
#'
#' If the log is less than 6 meters:
#' \deqn{V(m^3) = \frac{SED^2*L}{10000}}
#'
#' For logs equal or greater than 6 meters:
#' \deqn{V(m^3) = \frac{SED + (\frac{L' - 4}{2})^2 * L}{10000}}
#' where:
#' * SED = Small End Diameter in cm (rounded to the lower even centimeter). No decimal places.
#' * L = length in m (rounded to the lower decimeter). One decimal place (some places rounded to the lower even decimeter).
#' * L' = length in m (rounded to the lower meter). No decimal places.
#'
#' @return The selected function model.
#' @export
#' @import data.table
#' @importFrom matrixStats rowMins
#'
volform_selec <- function(func_name) {

  requireNamespace("data.table")
  iseven <- function(x) x %% 2 == 0  # Auxiliary function

  volform_functions <- list(
    geometric = function(x, l_length_wd) {
      # volFunc <- function(x, ...) {
      #     ds <- x[, !colnames(x) %in% "idseq"]
      #     v <- round(ds^2/40000 * pi * l_length_wd, 4)
      #     return(v)
      # }
      # ds <- as.matrix(x)
      return(data.table(round(x^2/40000 * pi * l_length_wd, 4)))
    }
  ,
    hoppus = function(x, l_length_wd) {
      # volFunc <- function(x, ...) {
      #     ds <- x[, !colnames(x) %in% "idseq"]
      #     v <- round(ds^2 * l_length_wd * 6.1685e-05, 4)
      #     return(v)
      # }
      # ds <- as.matrix(x)
      return(data.table(round(x^2 * l_length_wd * 6.1685e-05, 4)))
    }
  ,
    smalian = function(x, l_length_wd) {
      # x <- cut_diam_wd
      ds <- as.matrix(x)
      v <- round((ds[, -ncol(ds)]^2 + ds[, -1]^2) * l_length_wd * (pi/80000), 4)
      # v[v==0] <- 5000
      # v[v == rowMins(v)] <- 0  # por que el ultimo valor lo manda a cero?; ojo que esto puede que no funcione bien REVISAR!!
      # v[v==5000] <- 0
      return(data.table(v))
  }
  ,
    newton = function(x, l_length_wd) {
      # volFunc <- function(x, ...) {
      #     ds <- x[, !colnames(x) %in% "idseq"]
      #     led <- ds[seq_along(ds)[!iseven(seq_along(ds))][-length(seq_along(ds)[!iseven(seq_along(ds))])]]
      #     mid <- ds[seq_along(ds)[iseven(seq_along(ds))]]
      #     sed <- ds[seq_along(ds)[!iseven(seq_along(ds))][-1]]
      #     v <- mapply(function(x, y, z) round((x^2 + 4 * z^2 + y^2) * l_length_wd * (pi/240000), 4), x = led, y = sed, z = mid)
      #     v[which.min(v[v > 0])] <- 0
      #     return(v)
      # }
      # x <- cut_diam_wd
      # ds <- as.matrix(x[1, ])
      ds <- as.matrix(x)
      ncols <- ncol(ds)
      led <- ds[, seq(1:ncols)[!iseven(seq(1:ncols))]]
      led <- led[, -ncol(led)] # eliminar ultimo registro
      mid <- ds[, seq(1:ncols)[iseven(seq(1:ncols))]]
      sed <- ds[, seq(1:ncols)[!iseven(seq(1:ncols))][-1]]
      v <- round((led^2 + 4 * mid^2 + sed^2) * l_length_wd * (pi/240000), 4)
      # v[v==0] <- 5000
      # v[v == rowMins(v)] <- 0  # por que el ultimo valor lo manda a cero?; ojo que esto puede que no funcione bien. REVISAR!!
      # v[v==5000] <- 0
      return(data.table(v))
  }
  ,
    jas = function(x, l_length_wd) {
      xt <- trunc(x)  # rounding up to whole centimeters.
      xt <- (xt - (xt %% 2))  # rounding to the lower even centimeter.
      len <- trunc(l_length_wd * 10) / 10  # rounding up at the lower decimeter (not every 20cm!)
      if (is.matrix(l_length_wd)) {
        if (l_length_wd < 6) {
          v <- round(xt^2 * len/10000, 4)
        } else {
          v <- round((xt + ((floor(l_length_wd) - 4)/2))^2 * len/10000, 4)
        }
      } else {
        v <- matrix(0, nrow=nrow(xt), ncol=ncol(xt))
        p6_ <- l_length_wd < 6
        p6 <- l_length_wd >= 6
        if (sum(p6_) > 0) v[p6_] <- round(xt[p6_]^2 * len[p6_]/10000, 4)
        if (sum(p6) > 0) v[p6] <- round((xt[p6] + ((floor(l_length_wd[p6]) - 4)/2))^2 * len[p6]/10000, 4)
      }
      return(data.table(v))
    }
    #,
    # jas2 = function(x, l_length_wd) {
    #   # C. Ellis, J & H. Sanders, D & Pont, David. (1996). JAS log volume estimates of New Zealand radiata pine and Douglas-fir logs. 41.
    #   # https://www.researchgate.net/publication/242253100_JAS_log_volume_estimates_of_New_Zealand_radiata_pine_and_Douglas-fir_logs
    #
    #   # ds <- x[, !colnames(x) %in% "idseq"]
    #   # if (l_length_wd < 6)
    #   #     v <- round(ds^2 * l_length_wd/10000, 4)
    #   # if (l_length_wd >= 6)
    #   #     v <- round((ds + ((floor(l_length_wd) - 4)/2))^2 * l_length_wd/10000, 4)
    #   # return(v)
    #
    #   cround <- function(x, digits = 0) {
    #     a <- ifelse(x > 0, 0.5, -0.5)
    #     if (digits == 0) {
    #       floor(x + a)
    #     }
    #     else {
    #       m <- 10^digits
    #       floor(x * m + a)/m
    #     }
    #   }
    #
    #   if (funcSel == "jas") {
    #     if (lseg < 6) {
    #       volFunc <- function(x) {
    #         v <- cround(ifelse(x < 14, trunc(x), trunc(x/2) * 2)^2 * (trunc(lseg1/2) * 2)/10000, 4)
    #       }
    #     }
    #     else {
    #       volFunc <- function(x) {
    #         v <- cround((ifelse(x < 14, trunc(x), trunc(x/2) * 2) + ((floor((trunc(lseg1/2) * 2)) - 4)/2))^2 * (trunc(lseg1/2) * 2)/10000, 4)
    #       }
    #     }
    #   }
    #   # --------
    #   if (l_length_wd < 6) {
    #     v <- round(x^2 * l_length_wd/10000, 4)
    #   } else {
    #     v <- round((x + ((floor(l_length_wd) - 4)/2))^2 * l_length_wd/10000, 4)
    #   }
    #   return(data.table(v))
    # }
  )

  if (func_name %in% names(volform_functions)) {
    return(volform_functions[[func_name]])
  } else {
    stop(paste0("Invalid volform function name: ", func_name))
  }
}
