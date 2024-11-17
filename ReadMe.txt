Primero lo que tenemos que entender es que el robot se divide en X estados diferentes los cuales son:

	1.Caminando hacia el target: Este se compone de diferentes estados también ya que antes de llegar al objetivo hay una pared que 
	rodea, lo que se hace en este caso es moverse teniendo en cuanta la clásica formula de movimiento de física la cual X = X + V * t
	aplicándola en X y Z con el uso del Seno y Coseno para que se mueva siguiendo el forward teniendo en cuanta que el robot rota al 	intentar evadir la pared, mientras evade la pared encontramos 4 estados diferentes que hacen un lerp en la rotación del robot
	la primera rota al robot a una rotación dada para evitar el muro, la segunda rotación es para poner recto otra vez el robot para 
	que pase el muro, el tercero consiste en retornar el robot a su posición original rotándolo en los grados negativos de la primera
	rotación y el último consiste en volver a poner el robot recto, hasta que llega a una cierta distancia del target donde pasa al 	siguiente estado.

	2.Coguiendo el Objetivo: Este estado lo que hace es aplicar rotación en los 3 diferentes Joints a partir de unas amplitudes dadas	hasta que la distancia entre el último joint y el target es muy pequeña y da la sensación de que ya lo puede coger, tras esto
	se hace el target hijo del último joint y pasa al siguiente estado
	
	3.Recomponer el brazo: Este consiste en volver a poner el brazo en sus rotaciones originales para transportar el target a la mesa,	lo que hago aquí es, en cambio de sumar el tiempo y aplicarlo a las rotaciones, lo empiezo a restar para conseguir el movimiento	contrario, a la vez que impido rotarse a los joint si se pasan de su rotación original, porque si no empiezan a rotar unos más que	otros y no se consigue la rotación deseada, tras conseguir la posición final pasa al siguiente estado.

	4.Caminando hacia la mesa: Este hace lo mismo que el caminando hacia el target solo que en este caso como no hay ningún muro ni		nada, simplemente va hacia delante, hasta que se encuentra a una cierta distancia de la mesa y pasa al siguiente estado.

	5.Soltar: Este aplica la misma idea que el Consiguiendo el Objeto solo que en este caso hay ciertos valores distintos ya que ahora	lo que queremos es colocar el objeto sobre la mesa, diferenciando la amplitud del último joint y la distancia a la hora de soltar 
	el objeto, la cual tiene en cuenta el target y la mesa, a la vez, como en este ejercicio pedía que rotásemos el target para estar	horizontal, también se aplica un nuevo calculo para rotar tanto la X como la Y en el último joint para rotar en horizontal el 		target, tener en cuenta que en este momento debido a que solo queremos que el target rote en Z lo que hacemos es congelar las		demás axis de rotación del target haciendo que solo rote en Z para conseguir el efecto deseado

	6.Recomponer el brazo: Este estado es el mismo que el primero que recompone el brazo pero aplicando las amplitudes utilizadas a la	hora de soltar el objeto  