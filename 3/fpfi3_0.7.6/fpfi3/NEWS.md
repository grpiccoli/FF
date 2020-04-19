# Releases

## v0.7.1 (2018-08-24)

* Changed names for projects: 
    * eucalyptus pradaria renamed to eucalyptus brazil_ms
    * eucalyptus fds renamed to eucalyptus brazil_rs
    * eucalyptus_grandis uruguay_guanare renamed to uruguay_solid (original uruguay_solid in SimulateTreesHeight renamed uruguay_solid2)

* Taper changed: warner. Unknown variable `hr_m` inside warner was replaced with `cutm`. Not tested, changed as suggestion of the package checking when compiling.
* Added NULL dummy variables to several functions to avoid RMD notes when checking the package (ThinningFit, DoBucking, bbarplot, lmak, lpBuck, plotHist).


## v0.7.2 (2018-09-03)

* Added Pinus Radiata, New Zealand full usability


## v0.7.3 (2018-09-06)

* Added Max-Burkhart for Chile (using 5 parameters), `'mb_chile'` ([see it here](http://www.redalyc.org/pdf/629/62945379008.pdf)).
* Added Documentation/description skeleton for the package (as `fpfi3`).


## v0.7.4 (2018-09-06)

* Fixed bug in `EstimateDBHSd` function for pinus_radiata, new_zealand.


## v0.7.5 (2018-10-19)

* Auxiliary.R; EstimateDBHSd: changed log10 for log, as indicated in the [paper](http://www.ainfo.inia.uy/digital/bitstream/item/2876/1/15630021107224844.pdf). The output remains the same, as the there was one log10 in the numerator and one log10 in the denominator.
* MAJOR CHANGE: EstimateDBHSd function loops replaced for eucalyptus_grandis, uruguay_solid. Also added a new variable function call "qmds" (`qmds = Dg[i-1], Dg[i]`), for internal use for that species and region (compare computed sd with the variation coefficient and replace when necessary).
    * Solved TODO: modificar la función para que no use loop; dejarla del mismo tipo que el resto de Project... y asi aprovechar el loop de Growing y no generar otro interno extra. **RESUELTO**.
    * pinus_radiata; new_zealand: también fue transformado, pero no ha sido revisado.

## v0.7.6 (2018-10-23)
* growth.R; Growth: Fixed Dg calculation for thinning process (after last changes).
---
---
---
# TODO:

* Allow alt parameter in SiteIndex function, to be changed by the user.

# Notes asides:

Files and functions to check when adding a new country/region:

|     File    |        Function       |
|:-----------:|:---------------------:|
| auxiliary.R |     BafterThin        |
| auxiliary.R |     EstimateDBHSd     |
| auxiliary.R |       SiteIndex       |
| utilities.R | availableSpecies*     |
| sims.R      | SimulateTreesHeigth   |
| project.R   | GetBAAfterThinning    |
| project.R   | ProjectBasalArea      |
| project.R   | ProjectDominantHeight |
| project.R   | ProjectNTrees         |
| project.R   | ProjectVolume         |

