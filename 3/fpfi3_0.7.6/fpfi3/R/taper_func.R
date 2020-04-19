#' @title Taper form selector.
#' @md
#' @description Select the desired function.
#'
#' @param func_name A string indicating the model to be used. See \code{Details} for more information.
#' @param x A data.table data frame with the at least, with the dbh and ht. Every row is a different tree
#' @param coeffs A vector with the coefficients for the model to be used.
#' @param cut_ht A vector with the desired cut heights.
#' @param dbh A string, indicating the column name in \code{x} that containts the Diameter at Breast Height (dbh) of every tree.
#' @param ht A string, indicating the column name in \code{x} that containts the heigth (ht) of every tree.
#' @param hm A string or a scalar number. In the first case (string), it means the column in \code{x} that shows every tree commercial height.
#' If scalar number, it will be used as a common commercial heigth for all the trees. Default is NULL, which means that commercial heigth will not be used.
#'
#' @details The full list of implemented models are below. Default is "dss":
#' 1. \code{bruce}: the Bruce's model for pine.
#' See the [documentation](http://www.ingentaconnect.com/content/saf/fs/1968/00000014/00000003/art00021).
#' 2. \code{cao}: the modified segmented Max-Burkhart model developed by Cao.
#' 3. \code{cao2}: the modified segmented Max-Burkhart model implemented by FPFI.
#' See the [documentation](http://www.ingentaconnect.com/content/saf/fs/2015/00000061/00000002/art00002).
#' 4. \code{dss}: the modified segmented Max-Burkhart model implemented in DSS grandis (SAG grandis model).
#' See the [documentation](http://www.ainfo.inia.uy/digital/bitstream/item/2876/1/15630021107224844.pdf).
#' 5. \code{fang}: the Fang-Border-Bailey model.
#' See the [documentation](http://www.ingentaconnect.com/contentone/saf/fs/2000/00000046/00000001/art00002).
#' 6. \code{fds}: the 7th degree polynomial model developed by Granflor for FDS.
#' 7. \code{garay}: the Garay's taper model.
#' 8. \code{mb}: the segmented Max-Burkhart polynomial model.
#' See the [documentation](http://www.ingentaconnect.com/content/saf/fs/1976/00000022/00000003/art00011).
#' 9. \code{mb_chile}: the segmented Max-Burkhart polynomial model (adapted for Chile).
#' See model 17 in the [documentation](http://www.redalyc.org/pdf/629/62945379008.pdf).
#' 10. \code{mm}: the Muhairwe's model modified by Methol.
#' See the [documentation](http://www.scielo.edu.uy/pdf/agro/v18n2/v18n2a06.pdf).
#' 11. \code{parresol}: the Thomas and Parresol model for pine.
#' See the [documentation](http://www.nrcresearchpress.com/doi/abs/10.1139/x91-157?journalCode=cjfr#.WV7vylGQwqM).
#' 12. \code{poly5}: the 5th degree polynomial model.
#' See the [documentation](http://cerne.ufla.br/ojs/index.php/CERNE/article/view/604/513).
#' 13. \code{prad}: the 6th degree polynomial model developed by Granflor for PRAD.
#' 14. \code{sispinus}: the 4th degree polynomial model used in SisPinus.
#' 15. \code{warner}: See the [documentation](http://dx.doi.org/10.1016/j.anres.2016.04.005).
#'
#' @return A data.table dataframe with the tree diameter per every \code{cut_ht} and tree (rows)
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}, Christian Mora, \email{crmora@fibraconsult.cl}
#' @export
#' @import data.table
#'
taper_selec <- function(func_name, x, coeffs, cut_ht, dbh, ht, hm=NULL) {

  requireNamespace("data.table")

  # (x, coeffs, cut_ht, dbh, ht)
  taper_functions <- list(
    fang = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      I1 <- I2 <- matrix(0, ncol=cols, nrow=rows)
      I1[xr >= b[4] & xr <= b[5]] <- 1
      I2[xr > b[5] & xr <= 1] <- 1
      k <- pi/40000

      alpha1 <- (1 - b[4])^((b[2] - b[1]) * k/(b[1] * b[2]))
      alpha2 <- (1 - b[5])^((b[3] - b[2]) * k/(b[2] * b[3]))
      p0 <- 0.1/H
      t0 <- (1 - p0)^(k/b[1])
      t1 <- (1 - b[4])^(k/b[1])
      t2 <- (1 - b[5])^(k/b[2])
      term1 <- b[6] * D^b[7] * H^(b[8] - k/b[1])
      term2 <- b[1] * (t0 - t1) + b[2] * (t1 - alpha1 * t2) + b[3] * alpha1 * t2
      c1 <- sqrt(term1/term2)
      Beta <- b[1]^(1 - (I1 + I2)) * b[2]^I1 * b[3]^I2

      yhat <- c1 * sqrt(H^((k - b[1])/b[1]) * (1 - xr)^((k - Beta)/Beta) * alpha1^(I1 + I2) * alpha2^I2)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    dss = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0
      # yhat2 <- ifelse(xr > 1, 0, D * sqrt(b[1] * (xr - 1) + b[2] * (xr^2 - 1) + b[3] * log(D) *
      #                                      (b[5] - xr)^2 * (ifelse(xr < b[5], 1, 0)) + b[4] *
      #                                      log(D) * (b[6] - xr)^2 * (ifelse(xr < b[6], 1, 0))))
      p3 <- p2 <- p1 <- matrix(0, ncol=cols, nrow=rows)
      p1[xr <= 1] <- 1
      p2[xr < b[5]] <- 1
      p3[xr < b[6]] <- 1
      yhat <- p1 * D * sqrt(b[1] * (xr - 1) + b[2] * (xr^2 - 1) + b[3] * log(D) * (b[5] - xr)^2 * p2 + b[4] * log(D) * (b[6] - xr)^2 * p3)
      yhat[yhat < 0 | is.na(yhat)] <- 0
      return(round(yhat, 2))
    }
    ,
    bruce = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- (H - matrix(rep(hr, each=rows), nrow=rows))/(H - 1.3)
      # xr[xr < 0] <- 0
      yhat <- D * sqrt(b[1] * xr^1.5 + b[2] * (xr^1.5 - xr^3) * D + b[3] * (xr^1.5 - xr^3) * H + b[4] *
                          (xr^1.5 - xr^32) * D * H + b[5] * (xr^1.5 - xr^32) * H^0.5 + b[6] * (xr^1.5 - xr^40) * H^2)
      yhat[yhat < 0 | is.na(yhat)] <- 0
      return(round(yhat, 2))
    }
    ,
    cao = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- (H - matrix(rep(hr, each=rows), nrow=rows))/(H - 1.3)
      # xr[xr < 0] <- 0

      b1 <- 1 - b[1] - ifelse(b[4] < 1, 1, 0) * b[2] * (1 - b[4])^2 - ifelse(b[5] < 1, 1, 0) * b[3] * (1 - b[5])^2
      p2 <- p1 <- matrix(0, ncol=cols, nrow=rows)
      p1[b[4] < xr] <- 1
      p2[b[5] < xr] <- 1
      yhat <- D * sqrt((b1 * xr + b[1] * xr^2 + p1 * b[2] * (xr - b[4])^2 + p2 * b[3] * (xr - b[5])^2))

      yhat[yhat < 0 | is.na(yhat)] <- 0
      return(round(yhat, 2))
    }
    ,
    cao2 = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- (H - matrix(rep(hr, each=rows), nrow=rows))/(H - 1.3)
      # xr[xr < 0] <- 0

      b1 <- 1 - b[1] - ifelse(b[4] < 1, 1, 0) * b[2] * (1 - b[4])^2 * log(D) - ifelse(b[5] < 1, 1, 0) *  b[3] * (1 - b[5])^2 * log(D)
      p2 <- p1 <- matrix(0, ncol=cols, nrow=rows)
      p1[b[4] < xr] <- 1
      p2[b[5] < xr] <- 1
      yhat <- D * sqrt((b1 * xr + b[1] * xr^2 + p1 * log(D) * b[2] * (xr - b[4])^2 + p2 * log(D) * b[3] * (xr - b[5])^2))

      yhat[yhat < 0 | is.na(yhat)] <- 0
      return(round(yhat, 2))
    }
    ,
    fds = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # # b[c(7, 8)] <- 1
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      yhat <- D * (b[1] + b[2] * xr^1e-04 + b[3] * xr^0.01 + b[4] * xr^0.1 + b[5] * xr^1 + b[6] * xr^5 + b[7] * xr^10 + b[8] * xr^20)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    garay = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      yhat <- D * b[1] * (1 + b[2] * log(1 - b[3] * xr^b[4]))
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    mb = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      p1 <- p2 <- matrix(0, ncol=cols, nrow=rows)
      p1[xr < b[5]] <- 1
      p2[xr < b[6]] <- 1

      yhat <- D * sqrt(b[1] * (xr - 1) + b[2] * (xr^2 - 1) + b[3] * p1 * (b[5] - xr)^2 + b[4] * p2 * (b[6] - xr)^2)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    mb_chile = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      p1 <- p2 <- matrix(0, ncol=cols, nrow=rows)
      p1[xr < b[4]] <- 1
      p2[xr < b[5]] <- 1

      yhat <- D * sqrt(b[1] * (xr - 1) + b[2] * (xr^2 - 1) + b[3] * p1 * (b[4] - xr)^2 + b[2] * p2 * (b[5] - xr)^2)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    mm = function(x, b, cols, rows, D, H, xr, cutm) {
      C <- b[1] * log(xr + 0.001) + b[2] * exp(xr) + b[3] * (D/H) + b[4] * log(D) + b[5] * (H/sqrt(cutm)) + b[6] * (D/H)/cutm
      di <- b[7] * D^b[8] * b[9]^D * (1 - sqrt(xr))^C
      di[xr < 0 | xr > 1] <- 0
      di[is.na(di)] <- 0
      return(round(di, 2))
    }
    ,
    parresol = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      yhat <- D * (b[1] * (xr - 1) + b[2] * sin(b[4] * pi * xr) + b[3]/tan(pi * xr/2))
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    poly5 = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      yhat <- D * (b[1] + b[2] * xr + b[3] * xr^2 + b[4] * xr^3 + b[5] * xr^4 + b[6] * xr^5)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    prad = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      yhat <- D * (b[1] + b[2] * xr^0.01 + b[3] * xr^0.1 + b[4] * xr^1 + b[5] * xr^2 + b[6] * xr^10 + b[7] * xr^15)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    sispinus = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      yhat <- D * (b[1] + b[2] * xr + b[3] * xr^2 + b[4] * xr^3 + b[5] * xr^4)
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
    ,
    warner = function(x, b, cols, rows, D, H, xr, cutm) {
      # x <- as.matrix(void0)[1, , drop=F]
      # x <- void0
      # b <- as.numeric(coeffs)
      # hr <- cut_ht
      # cols <- length(hr)
      # rows <- nrow(x)
      #
      # D <- matrix(rep(x[[dbh]], cols), ncol=cols)
      # H <- matrix(rep(x[[ht]], cols), ncol=cols)
      # xr <- matrix(rep(hr, each=rows), nrow=rows)/H
      # xr[xr < 0] <- 0

      beta1 <- b[1] * H
      beta2 <- b[2]
      beta3 <- b[3]/H + b[4] * (D/10) + b[5] * (D/10)^2
      # S <- beta1 * beta2^2 * (1.3 - hr_m)/((1 + beta2 * hr_m) * (1 + beta2 * 1.3) * (1 + beta2 * H))
      S <- beta1 * beta2^2 * (1.3 - cutm)/((1 + beta2 * cutm) * (1 + beta2 * 1.3) * (1 + beta2 * H))

      # yhat <- (H - hr_m) * (S + beta3 * (hr_m - 1.3) + D/(H - 1.3))
      yhat <- (H - cutm) * (S + beta3 * (cutm - 1.3) + D/(H - 1.3))
      yhat[yhat < 0 | is.na(yhat) | xr > 1] <- 0
      return(round(yhat, 2))
    }
  )

  if (func_name %in% names(taper_functions)) {
    tap <- taper_functions[[func_name]]
  } else {
    stop(paste0("Invalid taper function name: ", func_name))
  }

  x <- data.table(x)
  b <- as.numeric(coeffs)
  cols <- length(cut_ht)
  rows <- nrow(x)

  D <- matrix(rep(x[[dbh]], cols), ncol=cols)
  H <- matrix(rep(x[[ht]], cols), ncol=cols)
  cutm <- matrix(rep(cut_ht, each=rows), nrow=rows)
  if (func_name %in% c("bruce", "cao", "cao2")) {
    xr <- (H - cutm)/(H - 1.3)
  } else {
    xr <- cutm/H
  }
  xr[xr < 0] <- 0  # Trees height
  ans <- tap(x, b, cols, rows, D, H, xr, cutm)

  # Commercial heigth
  if (!is.null(hm)) {
    if (is.numeric(hm)) {
      Hm <- matrix(rep(hm, cols*rows), ncol=cols)
    } else {
      Hm <- matrix(rep(x[[hm]], cols), ncol=cols)
    }
    ans[xr * H >= Hm] <- 0  # TODO: puede ser mejorado cuando hm es igual para todos; saca el numero de columnas fijas
  }

  colnames(ans) <- cut_ht
  return(data.table(ans))
}
