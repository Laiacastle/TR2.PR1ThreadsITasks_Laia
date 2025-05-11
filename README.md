# Enunciat 1 Comensals

## Solució proposada

he creat una 'cua' (queue) de Threads pero que tots el comensals menjin, cuan un comansal vol menjar primer pensa i després comprova si els sues palets estan disponibles, en cas de que no ho estiguin torna al final de la cua per esperar el seu torn, en cas de que pugui menjar bloquejarà els palets peroquè no hi ha hagi bloqueig.
Si algun comensal pasa fam mes de 15 segons el programa acaba, i en cas de que hagin passat 30 segons també termina.
he creat els palets com objectes i els he possat a una llista. a la funció de 'Menja' es passa l'id del comansal, i selecciona els palets amb el seu mateix id i un menys, meny en el cas del comensal 1 que hagafa el seu nuemero i el numero 5.
Cada vegada que algun comensal fa alguna cosa bloqueixo la consola.

## Dades d'anàlisis

__¿Quants comensals han passat fam?__

Ningú

__¿Quantes vegades ha menjat com máxim un comensal?__

96 vegades el comensal 5

__¿Quantes vegades ha sigut el mínim de vegades menjades?__

79 vegades, el comnsal 3

# Enunciat 2 Asteroides

## Solució proposada

He creat tres tasques un total sense comptar el Menu i el main.

__Tasca 1__

aquesta tasca es dedica a pintar la posició de tots el objectes, les vides, la nau, els asteroids, etc

__Tasca 2__

Aquesta calcula la posició del asteroids i de la nau, comprovant l'input de l'usuari en tot moment.

__Tasca 3__

Aquesta es una tasca que s'executa en segon pla, que simula se un analitzador de webs i fa que quan passin 30s s'acabi el programa automaticament.

### Funcionalitats

he utilitzats objectes per la nau i els asteroids amb un atribut per ambdues posicions (x, y); També he utilitzat el cancellationToken per cancelar les taskes en cas de que la nau colapsi contra algun asteroid.
He implementat un menu on pot jugar una partida, veure les estadistiques de les anteriors partides o sortir.
Quan s'acaba una partida automaticament guarda els resultats en un csv i torna el menu.
Les tres tasques s'executen al mateix temps i el main espera que alguna d'elles (Task.WhenAny()) s'acabi per mostrar els resultats i almatzenarlos.
En el joc constes de 3 vides (he intentat possar un cor pero no funcionaba).

# Biblogràfia

Queue<T> Clase. Api Microsoft. Recuperat el 5-4-2025 de https://learn.microsoft.com/es-es/dotnet/api/system.collections.generic.queue-1?view=net-8.0

CancellationTokenSource Clase. Api Microsoft. Recuperat el 10-4-2025 de https://learn.microsoft.com/es-es/dotnet/api/system.threading.cancellationtokensource?view=net-8.0

Convertir hercios en miliseconds. UnitJuggler. Recuperat el 9-4-2025 de https://www.unitjuggler.com/frequency-convertir-Hz-a-ms(p).html

La instrucción lock: asegúrese del acceso exclusivo a un recurso compartido. Api Microsoft. Recuperat el 6-4-2025 de https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/statements/lock

__ChatGPT:__
Promp: como puedo ponerle sonido a mi programa de c#?
Respuesta:
En C#, puedes reproducir sonidos de varias formas, dependiendo de qué tipo de sonido quieres usar y en qué entorno estás ejecutando tu programa (como consola o Windows Forms). Aquí van las formas más comunes:

✅ 1. Reproducir un beep simple (funciona en consola)
csharp
Copy code
Console.Beep(); // Suena con el tono y duración predeterminados
Puedes personalizar el tono (frecuencia en Hz) y la duración (en milisegundos):

csharp
Copy code
Console.Beep(1000, 500); // Frecuencia de 1000 Hz durante 500 ms
⚠️ Nota: En algunas consolas (Linux/macOS o terminales modernas), Console.Beep() podría no funcionar o estar silenciado.

