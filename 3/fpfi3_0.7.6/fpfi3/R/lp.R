#' @rdname lpBuck
#' @title Stem bcking simulation and merchantable volume calculation by linear model
#' @description lpBuck simulates a stem bucking process based on a list of products of differents lengths, using an integer lineal model in the process.
#'
#' @param treesDiam A data.table or matrix, where every row corresponds to a tree and every column to a diameter at a given bin.
#' @param treesDat A data.table dataframe, with colnames idseq, ht and dbh at least. Every row corresponds to a tree (in the same order as treeVol).
#' @param products  A data.frame of 4 columns and n rows, with product class information.
#' Column 1 is the small-end diameter ("diameter"), column 2 is product length ("length") that could be different for each one,
#' columnn 3 is the value ("value") of the log and column 4 is the product name ("name").
#' The order is not relevant, as logs with more value will be selected automatically by the solver.
#' @param hp Prunning height. Could be NULL (no prunning), a scalar indicating a common hp or
#' a character indicating the column in data that corresponds to every tree hp. Default is NULL (NOT IMPLEMENTED).
#' @param lseg0 Segment size (in m) for the volume estimation using the trapezoid aproximation. Default is 0.2 (NOT IMPLEMENTED).
#' @param volform Volume formula to use. Currently log volumes are calculated using the Smalian formula ("smalian"),
#' Newton formula ("newton"), geometric formula ("geometric"), Hoppus formula ("hoppus"), and JAS formula ("jas").
#' See \code{\link{volform_selec}} for more details. Default is "smalian".
#' @param grade_wd If TRUE, logs are graded after applying the discount to the middle girth. Default is TRUE.
#' @param mg_disc Discount in cm to be applied to the middle girth of the logs. Default is NULL (no discount).
#' @param length_disc Discount in cm to be applied to the merchantable lengths of the logs. Default is NULL (no discount).
#'
#' @details This functions is designed to be used by \code{\link[fpfi3]{DoBucking}}. Be carefull to give all the arguments in the proper as no major
#' checks will be performed (all checks and data preparation are made by the \code{\link[fpfi3]{DoBucking}} function).
#' @author Álvaro Paredes, \email{alvaro.paredes@fpfi.cl}
#' @return A data.table dataframe.
#' @export
#' @import data.table
#' @import lpSolve
#' @importFrom plyr count
#' @importFrom stats pnorm pweibull qnorm qweibull setNames
# @examples
lpBuck <- function (treesDiam, treesDat, products, hp = NULL, lseg0 = 0.2,
                    volform = "smalian",
                    grade_wd = FALSE, mg_disc = NULL, length_disc = NULL) {

  requireNamespace("lpSolve")

  # To avoid RMD checks notes:
  frag <- NULL; val_frag <- NULL; value <- NULL; product <- NULL

  # treesDat <- void0; treesDiam <- cut_diam
  volform_selected <- volform
  products <- as.data.table(products)
  products <- products[order(products[, "diameter"], decreasing=T), ] # hay que ordenar por diametro de mas grande a menor
  products[, frag:= length / (mGCD(round(products$length*100))/100)]
  if (volform_selected %in% c("newton", "hoppus", "geometric")) {
    if (sum(products[ , "length"] %% 2) > 0) {
      products[, frag:=frag*2] # si hay numeros impares, no podremos tener la "mitad" del log, por lo que hay que multiplicar por 2 para evitar ese problema
    }
  }
  products[, val_frag:= value/frag]
  # En teoria, deberia multiplicar frag (tomar el doble de fragmentos por log) si l_length fue dividido por dos en la funcion DoBucking, pero no es necesario.
  # La funcion lp reconocera esta situacion y permitira haara los ajustes pertinenes. La razon de esto, es que a mayor numero de frag por trozo, se reducen los
  # casos de arboles que tengan las mismas combinaciones de trozado, con lo cual se hace mas lento el proceso de optimizacion
  nproducts <- nrow(products)

  if (volform_selected == "newton") {
    treesDiam_ind <- seq(1, ncol(treesDiam), by=2)
  } else {
    treesDiam_ind <- 2:ncol(treesDiam) # parte en 2, porque el primero es el diametro a la altura de corte, que seria el diametro mayor y no el menor
  }

  tlist <- list()
  if (volform_selected %in% c("hoppus", "geometric")) {
    for (i in 1:nrow(products)) {
      tlist[[i]] <- rowSums(treesDiam[, treesDiam_ind, with=F] >= products$diameter[i]) + products$frag[i]/2 # como se mide en la mitad, hay mayor capacidad
    }
  } else {
    for (i in 1:nrow(products)) {
      tlist[[i]] <- rowSums(treesDiam[, treesDiam_ind, with=F] >= products$diameter[i])
    }
  }

  tbins <- t(do.call("rbind", tlist))  # bins disponibles por producto (fila) y arbol (columnas), transpuesto
  maxBins <- matrix(rep(rowSums(treesDiam[, treesDiam_ind, with=F] > 0), ncol(tbins)), ncol=ncol(tbins))
  tbins[tbins > maxBins] <- maxBins[tbins > maxBins] # revisando que no nos pasemos del numero maximo de bins (que no consideren bins iguales a 0 como disponibles)
  tbins <- cbind(1:length(tlist[[1]]), tbins) # agregando id
  colnames(tbins) <- c("id", letters[1:nproducts])
  # El siguiente paso tiene por objetivo, solamente ordenar la matriz tal cual la va a dejar plyr::count; de esa forma se puede rearmar toda
  # la estructura una vez pasada por el lp, sin perder el orden inicial, que es fundamental
  tbins <- tbins[with(as.data.frame(tbins), eval(parse(text=paste0("order(", paste(letters[1:nproducts], collapse= ','), ")")))), ]
  tbins_ids <- tbins[, 1]
  tbins_r <- plyr::count(tbins[, -1])   # casos unicos, para disminuir el numero de veces que corre el lp
  bins_freq <- tbins_r[, ncol(tbins_r)]   # frecuencias de casos unicos
  tbins_r <- tbins_r[, -ncol(tbins_r)]  # frecuencias fuera de la matrix

  idx <- setNames(c(which(c(diff(products$diameter), 0) != 0), nproducts), unique(products$diameter))
  idx <- idx[as.character(products$diameter)]
  rest_mat <- matrix(rep(products$frag, each=nproducts), nrow=nproducts)  # numero de bins necesarios para completar 1 producto, por producto
  rest_mat2 <- rest_mat
  rest_mat2[] <- 0
  # Por diametro, los productos mas grandes no tienen restriccion con respecto a los menos anchos; por el contrario, mientras mas pequeno el diametro,
  # mas factible es que pueda obtenerse ese producto de cualquier lugar del arbol.
  # Hay que considerar que tener presente que los productos mas grandes deben considerarse en la restriccion de bins maximos. Eso es rest_mat2
  for (i in 1:nproducts) {
    rest_mat2[i, 1:idx[i]] <- rest_mat[i, 1:idx[i]]
  }

  f.obj <- products$value           # precios de los productos
  f.con <- rest_mat2                # matriz de restricciones
  f.dir <- rep("<=", nrow(f.con))   # tipo de restriccion
  f.typ <- 1:length(f.obj)          # indices de las variables (productos) que seran enteras

  n_products <- matrix(NA, ncol=nrow(products), nrow=nrow(tbins_r), dimnames=list(NULL, products$name))
  for (i in 1:nrow(tbins_r)) {
    f.rhs <- as.vector(tbins_r[i, ])   # bins disponibles por producto (segun diametro)
    lpl <- lp("max", f.obj, f.con, f.dir, f.rhs, int.vec=f.typ)
    n_products[i, ] <- lpl$solution
  }

  # Expanding to the original dimensions
  lp_out <- n_products[rep(seq_len(nrow(n_products)), bins_freq), ]

  # Volume calculation an all
  mprod <- max(rowSums(n_products)) # maximo numero de productos obtenidos desde un unico arbol (revisandolos todos)

  # TODO: ver si se puede sacar el loop dentro de este if y vectorizarlo.... deberia poderse
  if (volform_selected == "newton") {
    # se suma uno porque tambien necesitamos el diametro mayor de la troza y se multiplica x 2 porque necesitamos el diametro medio tambien
    volb <- matrix(0, ncol=mprod * 2 + 1, nrow=nrow(tbins))
    vols <- volp <- prix <- matrix(0, ncol=mprod + 1, nrow=nrow(tbins))  # se suma uno porque tambien necesitamos el diametro mayor de la troza
    for (i in 1:nrow(lp_out)) {
      v <- rep(1:6, lp_out[i, ]) # ubicacion del numero de producto cada arbol (fila) y se multiplica
      # Se agrega 1 porque necesitamos el diametro mayor para cubicar, junto con el menor y el intermedio
      myv <- sort(unique(c(1, cumsum(products$frag[v])*2, cumsum(products$frag[v])*2 - products$frag[v])))
      # y tambien una matriz con los diametros respectivos
      volb[i, ] <- c(unlist(treesDiam[tbins_ids[i], myv, with=F]), rep(0, ncol(volb) - length(myv)))
      vols[i, ] <- c(0, products$length[v], rep(0, mprod - length(v))) # matriz con los largos
      # prix[i, ] <- c(0, products$value[v], rep(0, mprod - length(v)))
      volp[i, ] <-  c(0, v, rep(0, mprod - length(v))) # indica el producto que es
    }
  } else if (volform_selected %in% c("hoppus", "geometric")){
    volb <- vols <- volp <- prix <- matrix(0, ncol=mprod + 1, nrow=nrow(tbins))  # se suma uno porque tambien necesitamos el diametro mayor de la troza?
    for (i in 1:nrow(lp_out)) {
      v <- rep(1:6, lp_out[i, ]) # ubicacion del numero de producto cada arbol (fila) y se multiplica
      # Se agrega 1 porque necesitamos el diametro mayor para cubicar, junto con el menor
      # volb[i, ] <- c(unlist(treesDiam[i, c(1, cumsum(products$frag[v])), with=F]), rep(0, mprod - length(v)))
      # y tambien una matriz con los diametros respectivos
      volb[i, ] <- c(unlist(treesDiam[tbins_ids[i], c(1, cumsum(products$frag[v]) + 1 - products$frag[v]/2), with=F]) , rep(0, mprod - length(v))) # diametro central
      vols[i, ] <- c(0, products$length[v], rep(0, mprod - length(v))) # matriz con los largos
      # prix[i, ] <- c(0, products$value[v], rep(0, mprod - length(v)))
      volp[i, ] <-  c(0, v, rep(0, mprod - length(v))) # indica el producto que es
    }
  } else {
    volb <- vols <- volp <- prix <- matrix(0, ncol=mprod + 1, nrow=nrow(tbins))  # se suma uno porque tambien necesitamos el diametro mayor de la troza
    for (i in 1:nrow(lp_out)) {
      v <- rep(1:6, lp_out[i, ]) # ubicacion del numero de producto cada arbol (fila) y se multiplica
      # Se agrega 1 porque necesitamos el diametro mayor para cubicar, junto con el menor
      # volb[i, ] <- c(unlist(treesDiam[i, c(1, cumsum(products$frag[v])), with=F]), rep(0, mprod - length(v)))
      # y tambien una matriz con los diametros respectivos
      volb[i, ] <- c(unlist(treesDiam[tbins_ids[i], c(1, cumsum(products$frag[v]) + 1), with=F]), rep(0, mprod - length(v)))
      vols[i, ] <- c(0, products$length[v], rep(0, mprod - length(v))) # matriz con los largos
      # prix[i, ] <- c(0, products$value[v], rep(0, mprod - length(v)))
      volp[i, ] <-  c(0, v, rep(0, mprod - length(v))) # indica el producto que es
    }
  }

  if (volform %in% c("newton", "smalian")) {
    runlp <- volform_selec(volform_selected) (volb, vols[, -1])  ## volumenes de cada troza
  } else if (volform %in% c("geometric", "hoppus", "jas")) {
    runlp <- volform_selec(volform_selected) (volb[, -1], vols[, -1])  ## volumenes de cada troza, descontando la base
  } else {
    stop(paste0("fpfi2:lp. volform parameter ", volform, " is not valid"))
  }

  mapp <- setNames(products$name, 1:nrow(products))
  vf <- data.table(idseq = rep(tbins_ids, mprod),
                   log = rep(1:mprod, each=nrow(volb)),
                   diameter = c(volb[, -1]),
                   volume = unlist(runlp),
                   product = c("", as.character(products$name))[c(volp[, -1]) + 1]
                   )
  vf <- vf[product != "", ]
  void1 <- merge(x = vf, y = treesDat, by = "idseq", all.x = TRUE)

  return(void1)
}



