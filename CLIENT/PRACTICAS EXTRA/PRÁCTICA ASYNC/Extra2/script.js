// Crea un programa que realice una petición GET a través de un servidor web a un archivo JSON que 
// almacena un listado de usuarios. Para ello, crea una carpeta que almacene el archivo HTML que 
// utilizarás para ejecutar el programa y el archivo JSON indicado y utiliza la extensión Live 
// Server de Visual Studio Code para ejecutar un servidor sobre dicha carpeta.

//  Si la petición tarda más de 5 segundos, se deberá cancelar y deberá mostrarse un mensaje en la 
// consola; si la petición tiene éxito, se deberá mostrar en la consola el listado de usuarios de la 
// siguiente manera:

    //  Usuario: Pérez, Laura.
    //  Usuario: Martínez, Ana.
    //  Usuario: González, Juan.

//     En primer lugar, intenta mostrar por la consola los datos recibidos.

//     Observa qué formato de datos devolverá la petición.

//     Una vez que hayas podido mostrar los datos en la consola, trata de formatear la salida, de acuerdo 
//     con las instrucciones.

//     Cuando esté todo funcionando, añade una señal para poder cancelar la petición.

//     Por último, añade el código para que la señal de abortar se active cuando pasen 5 segundos. Para 
//     ello, puedes utilizar un temporizador

let controller = new AbortController();
let signal = controller.signal;

fetch('users.json', { signal })
    .then(response => response.json())
    .then(data => {
        data.forEach(user => {
            console.log(`Usuario: ${user.apellido}, ${user.nombre}.`);
        });
    })
    .catch(error => {
        if (error.name === 'AbortError') {
            console.log('La petición ha sido cancelada debido a que ha tardado más de 5 segundos.');
        } else {
            console.error('Error en la petición:', error);
        }
    });

setTimeout(() => controller.abort(), 5000);