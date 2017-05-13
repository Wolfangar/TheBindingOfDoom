# TheBindingOfDoom
Procedural generation of Doom 2's levels
###### Bug connu
La salle "rose" se superpose à celle de gauche. L'algorithme ne sait pas que la salle de droite est trop "grande".
Il ne compare que la compatibilité entre le nombre et ancres/positions des portes.
Il s'agit ici de bien prévoir l'emboitement des prefabs.
![alt bug](https://github.com/Wolfangar/TheBindingOfDoom/blob/master/bug.png?raw=true)
