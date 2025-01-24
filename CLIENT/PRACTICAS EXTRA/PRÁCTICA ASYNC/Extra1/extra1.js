// TRABAJO CON UNA API DE SERVIDOR

//  Crea una página web que utilice un formulario para mostrar el login y la 
// imagen de avatar de un usuario de GitHub. El formulario tendrá un campo 
// de texto para introducir el nombre de usuario y un botón de envío. Se 
// utilizará la API de GitHub. 

// La URL que se debe utilizar es https://api.github.com/users/{USER}, siendo {USER} el nombre de usuario. 

// Por ejemplo, los datos del usuario vuejs están disponibles en https://api.github.com/users/vuejs (petición GET).

//  Los datos devueltos por la API están en formato JSON. Del objeto devuelto se utilizarán las propiedades login y avatar_url. 
// Se debe indicar si el usuario no existe.


document.getElementById('github-form').addEventListener('submit', function(event) {
    event.preventDefault();
    const username = document.getElementById('username').value;
    fetch(`https://api.github.com/users/${username}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('User not found');
            }
            return response.json();
        })
        .then(data => {
            document.getElementById('user-info').style.display = 'block';
            document.getElementById('error-message').style.display = 'none';
            document.getElementById('user-login').textContent = data.login;
            document.getElementById('user-avatar').src = data.avatar_url;
        })
        .catch(error => {
            document.getElementById('user-info').style.display = 'none';
            document.getElementById('error-message').style.display = 'block';
        });
});