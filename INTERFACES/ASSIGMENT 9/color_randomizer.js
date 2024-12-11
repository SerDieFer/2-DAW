// RANDOM COLOR GENERATOR FUNCTION
function randomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

// FUNCTION TO GET COMPLEMENTARY COLOR
function getComplementaryColor(color) {
    // REMOVE THE '#' CHARACTER AND CONVERT HEX TO RGB
    const hex = color.substring(1);
    const r = 255 - parseInt(hex.substring(0, 2), 16);
    const g = 255 - parseInt(hex.substring(2, 4), 16);
    const b = 255 - parseInt(hex.substring(4, 6), 16);
    return `rgb(${r}, ${g}, ${b})`;
}

// FUNCTION TO GENERATE RANDOM COLORS FOR GRADIENTS, SHADOWS, AND BORDERS
function generateRandomColors() {
    // SET NEW RANDOM COLOR VALUES FOR GRADIENTS
    document.documentElement.style.setProperty('--gradient-cabecera', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    document.documentElement.style.setProperty('--gradient-menu', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    document.documentElement.style.setProperty('--gradient-nav1', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    document.documentElement.style.setProperty('--gradient-nav2', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    document.documentElement.style.setProperty('--gradient-banner', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    document.documentElement.style.setProperty('--gradient-cuerpo1', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    document.documentElement.style.setProperty('--gradient-pie', `radial-gradient(circle, ${randomColor()}, ${randomColor()})`);
    
    // SET NEW RANDOM SHADOW COLORS
    const shadowCabecera = randomColor();
    const shadowMenu = randomColor();
    const shadowNav1 = randomColor();
    const shadowNav2 = randomColor();
    const shadowBanner = randomColor();
    const shadowCuerpo1 = randomColor();
    const shadowPie = randomColor();

    document.documentElement.style.setProperty('--shadow-cabecera', `0 0 10px ${shadowCabecera}, 0 0 20px ${shadowCabecera}`);
    document.documentElement.style.setProperty('--shadow-menu', `0 0 10px ${shadowMenu}, 0 0 20px ${shadowMenu}`);
    document.documentElement.style.setProperty('--shadow-nav1', `0 0 10px ${shadowNav1}, 0 0 20px ${shadowNav1}`);
    document.documentElement.style.setProperty('--shadow-nav2', `0 0 10px ${shadowNav2}, 0 0 20px ${shadowNav2}`);
    document.documentElement.style.setProperty('--shadow-banner', `0 0 10px ${shadowBanner}, 0 0 20px ${shadowBanner}`);
    document.documentElement.style.setProperty('--shadow-cuerpo1', `0 0 10px ${shadowCuerpo1}, 0 0 20px ${shadowCuerpo1}`);
    document.documentElement.style.setProperty('--shadow-pie', `0 0 10px ${shadowPie}, 0 0 20px ${shadowPie}`);
    
    // SET NEW RANDOM BORDER COLORS
    const borderCabecera = randomColor();
    const borderMenu = randomColor();
    const borderNav1 = randomColor();
    const borderNav2 = randomColor();
    const borderBanner = randomColor();
    const borderCuerpo1 = randomColor();
    const borderPie = randomColor();

    document.documentElement.style.setProperty('--border-cabecera', borderCabecera);
    document.documentElement.style.setProperty('--border-menu', borderMenu);
    document.documentElement.style.setProperty('--border-nav1', borderNav1);
    document.documentElement.style.setProperty('--border-nav2', borderNav2);
    document.documentElement.style.setProperty('--border-banner', borderBanner);
    document.documentElement.style.setProperty('--border-cuerpo1', borderCuerpo1);
    document.documentElement.style.setProperty('--border-pie', borderPie);

    // SET COMPLEMENTARY COLORS FOR HOVER EFFECT
    document.documentElement.style.setProperty('--hover-shadow-cabecera', `0 0 10px ${getComplementaryColor(shadowCabecera)}, 0 0 20px ${getComplementaryColor(shadowCabecera)}`);
    document.documentElement.style.setProperty('--hover-shadow-menu', `0 0 10px ${getComplementaryColor(shadowMenu)}, 0 0 20px ${getComplementaryColor(shadowMenu)}`);
    document.documentElement.style.setProperty('--hover-shadow-nav1', `0 0 10px ${getComplementaryColor(shadowNav1)}, 0 0 20px ${getComplementaryColor(shadowNav1)}`);
    document.documentElement.style.setProperty('--hover-shadow-nav2', `0 0 10px ${getComplementaryColor(shadowNav2)}, 0 0 20px ${getComplementaryColor(shadowNav2)}`);
    document.documentElement.style.setProperty('--hover-shadow-banner', `0 0 10px ${getComplementaryColor(shadowBanner)}, 0 0 20px ${getComplementaryColor(shadowBanner)}`);
    document.documentElement.style.setProperty('--hover-shadow-cuerpo1', `0 0 10px ${getComplementaryColor(shadowCuerpo1)}, 0 0 20px ${getComplementaryColor(shadowCuerpo1)}`);
    document.documentElement.style.setProperty('--hover-shadow-pie', `0 0 10px ${getComplementaryColor(shadowPie)}, 0 0 20px ${getComplementaryColor(shadowPie)}`);

    document.documentElement.style.setProperty('--hover-border-cabecera', getComplementaryColor(borderCabecera));
    document.documentElement.style.setProperty('--hover-border-menu', getComplementaryColor(borderMenu));
    document.documentElement.style.setProperty('--hover-border-nav1', getComplementaryColor(borderNav1));
    document.documentElement.style.setProperty('--hover-border-nav2', getComplementaryColor(borderNav2));
    document.documentElement.style.setProperty('--hover-border-banner', getComplementaryColor(borderBanner));
    document.documentElement.style.setProperty('--hover-border-cuerpo1', getComplementaryColor(borderCuerpo1));
    document.documentElement.style.setProperty('--hover-border-pie', getComplementaryColor(borderPie));
}

// CALL THE RANDOMIZER FUNCTION ON PAGE LOAD
window.onload = generateRandomColors;

// OPTIONALLY, YOU CAN TRIGGER COLOR CHANGES ON A BUTTON CLICK OR ANY OTHER EVENT
document.getElementById('change-colors-button')?.addEventListener('click', generateRandomColors);
