mweibull <- function(mean, stdev){
  # Returns the Method-of-Moments shape and scale parameter estimates
  # for the Weibull distribution, given a mean and variance.
  # Method:  Garcia, O.  NZ Journal of Forestry Science
  # Use help("dweibull") for definitions and more details.

  cv <- stdev / mean
  if (cv > 1.2 || cv <= 0) { # diferencia 1 (cv >= 1.2 || cv <= 0)
    print(c(stdev, mean))
    stop("Out of range")
  }
  a <- 1 / (cv * (((((0.007454537 * cv * cv - 0.08354348) * cv + 0.153109251) *
                      cv - 0.001946641) * cv - 0.22000991) * (1 - cv) * (1 - cv) + 1))
  return(list(shape = a, scale = mean / gamma(1 + 1 / a)))
}


#' @rdname availableSpecies
#' @title List or check the available list of species.
#' @description ..
#'
#' @param species A character indicating one of the species name. The function is case insensitive.
#' @param region A character indicating one of the region names (depending on the species). The function is case insensitive. Default is NULL.
#' @param callFunction A character indicating the function caller. For internal use. Default is NULL.
#'
#' @details To be completed.
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return When called without parameters, return a matrix with all the available species names and region. If called internally by a function,
#' (in order to check the species and region), it will trough an error if the species and region are not available, otherwise the function will return nothing.
#' @examples
#' availableSpecies()  # show all the species available.
#' availableSpecies('eucalyptus')   # check if it's available.
#' \dontrun{
#' # check if it's available. It doesn't exist, so an error will be triggered.
#' availableSpecies('test')
#' }
#' @export
#'
availableSpecies <- function(species=NULL, region=NULL, callFunction=NULL) {
  # available <- c("eucalyptus", "pine", "pinus_radiata", "pinus_taeda", "teak")
  available <- c("eucalyptus", "eucalyptus_grandis", "eucalyptus_globulus", "eucalyptus_nitens",
                 "gmelina",
                 "pine", "pinus_taeda", "pinus_radiata",
                 "teak")

  blocks <- list(eucalyptus = c("brazil_ms", "uruguay", "brazil_rs", ''),
                 eucalyptus_grandis = c("uruguay_pulp", "uruguay_solid", "uruguay_solid2", "brazil_ms", "brazil_rs", ''),
                 eucalyptus_globulus = c("uruguay", "chile", ""),
                 eucalyptus_nitens = c('chile', ''),
                 gmelina = c('ecuador', ''),
                 pine='',
                 pinus_radiata=c('chile', "new_zealand", ''),
                 pinus_taeda=c('uruguay', 'argentina', 'south-east-usa', ''),
                 teak=c('brazil', 'ecuador', 'nicaragua', 'panama', ''))

  if (is.null(species)) {
    pp <- rbindlist(lapply(1:length(blocks), function(x) {data.frame(species=names(blocks)[x], region=blocks[[x]])}))
    pp <- pp[region!='', ]
    return(pp)
  } else {
    species <- tolower(species)
    if (is.null(callFunction)) {
      cf <- ""
    } else {
      cf <- paste0("\nCalled from: ", callFunction, ".")
    }
    if (species %in% available) {
      if (length(region) == 0) {
        rg <- ''
      } else {
        if (region %in% blocks[[species]]) {
          rg <- tolower(region)
        } else {
          stop(paste0("Selected region ", region, " is not available for the selected species :", species,
                      "(use availableSpecies() to view the availables species and regions).", cf))
        }
      }
    } else {
      stop(paste0("Selected species ", species, " is not available (use availableSpecies() to view the available species).", cf))
    }
  }
  return(list(species=tolower(species), region=rg))
}


#' @rdname Dispatcher
#' @title Dispatcher function to select one of the growth internal functions according to the species and region.
#' @description This function have been designed to be used as internal. The Site Index, DBH sd estimation,
#' height simulation... are the functions that utilize this dispatcher to select the appropiate parameters and sub-functions according to the
#' species and region
#'
#' @param species A character indicating the species.
#' @param region A character indicating the region/type.
#' @param lista A list of functions, with structure \code{list(species=list(region))}. If region is NULL, first item inside the species will be used.
#' @param ... Other arguments passed to the selected function.
#' @inheritParams availableSpecies
#'
#' @details To be completed.
#' @family Internals
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @keywords internal
#' @export
#'
Dispatcher <- function(species, region, lista, callFunction='', ...) {
  if (class(species) == 'list') {
    fg <- with(species, availableSpecies(species, region, callFunction))
  } else {
    fg <- availableSpecies(species, region, callFunction)
  }
  sp <- fg$species
  rg <- fg$region

  pos <- ifelse(rg == '', 1, rg)
  if (class(species) == 'list') {
    if (is.null(lista[[sp]][[pos]])) {
      stop(paste0("Function ", callFunction, " not available for species ", sp, " and region ", rg))
    }
    ans <- do.call(lista[[sp]][[pos]], species)
  } else {
    ans <- lista[[sp]][[pos]](...)
  }
  return(ans)
}


#' @rdname print.FullSimulation
#' @title FullSimulation Printing Options
#' @description \code{print.FullSimulation} allow a better visualization that the normal \code{print}.
#' It shows a summary of the available information and not all the list components in a single print.
#'
#' @param x A \code{FullSimulation} object (generated by the \code{\link[fpfi3]{FullSimulation}} function.)
#' @param ... Other arguments ultimately passed to format.
#'
#' @details To be completed.
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @keywords internal
#' @export
#'
print.FullSimulation <- function(x, ...) {
  if (class(x) != "FullSimulation") {
    stop("Input is not a FullSimulation class")
  }
  nstands <- nrow(x$input$initial_conditions)
  nproducts <- nrow(x$input$products)
  cl <- as.list(x$cl)

  cat(paste0("\n",
             "FullSimulation object, with ", nstands, " stands and ", nproducts," products.\n",
             "\nSpecies: ", cl$species,
             "\nRegion: ", cl$region,
             "\nTaper Model: ", cl$model,
             "\nVolume Function: ", cl$volform,
             "\nDoBucking: ", cl$way,
             "\n\nUse this$simulation to acces the full table with the growth information,",
             "\n$taper to access the detailed or general list of yield and thinning tables",
             "\n\nExtra information could be extracted. See ?FullSimulation to view the detailed",
             "\nstructure of the returned list.",
             "\n"))
}
