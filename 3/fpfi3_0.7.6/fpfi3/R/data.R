#' fpfi3: Forestry toolbox for simulation.
#'
#' Growth models, Taper Models, Volume functions and Bucking (with/out LP). Include some plot functions to visualize data.
#'
#' @section Bucking:
#' Functions
#'
#' @section Plots:
#' Functions
#'
#' @section Simulation:
#' Functions
#'
#' @section Workflow:
#' Functions
#'
#' @docType package
#' @name fpfi3
NULL

#' Initial Conditions
#'
#' An example of the initial conditions for 5 stands.
#'
#' @format A \code{data.frame}, \code{data.table} with 20 variables:
#' \itemize{
#' \item{\code{id: }}{A unique numeric identificator.}
#' \item{\code{macrostand: }}{A character indicating the stand name.}
#' }
#' @keywords internal
#' @name initial_conditions
"initial_conditions"

#' Products
#'
#' An example of a set of products.
#'
#' @format A \code{data.table} with 4 variables:
#' \itemize{
#' \item{\code{diamater: }}{Small end diameter in cm.}
#' \item{\code{length: }}{Log length in meters.}
#' \item{\code{value: }}{The value (in any currency).}
#' \item{\code{name: }}{The log name.}
#' \item{\code{log_type: }}{A character indicating if the log is prunned ('p') or not prunned ('u').}
#' \item{\code{class: }}{An aggregation name, in case is necessary (for example,
#' when \code{link[fpfi3]{FullSimulation}} is ran with the options \code{byClass = TRUE}.}
#' }
#' @keywords internal
#' @name demo_products
"demo_products"


#' Coefficients
#'
#' An example of the taper coefficients.
#'
#' @format See \code{\link[fpfi3]{taper_selec}}.
#' @keywords internal
#' @name taper_coeffs
"taper_coeffs"


