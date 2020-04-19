var parse = require('parse-svg-path')
var extract = require('extract-svg-path')
 
var path = extract(__dirname +'/fpfi-logo-sq.svg')
var svg = parse(path)
console.log(svg)