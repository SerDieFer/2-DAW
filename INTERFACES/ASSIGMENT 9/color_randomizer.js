const elementStyle = document.documentElement.style;

// PRIMARY, SECONDARY, and TERTIARY COLOR GENERATOR FUNCTIONS
const primaryColor = () => {
    const primaryColors = ['#FF0000', '#FFA500', '#FFFF00', '#008000', '#0000FF', '#4B0082', '#8A2BE2'];
    const randomIndex = Math.floor(Math.random() * primaryColors.length);
    return primaryColors[randomIndex];
};

const secondaryColor = (primary) => {
    const secondaryColors = {
        '#FF0000': '#FF7F7F',  // Red -> Light Red
        '#FFA500': '#FFCC80',  // Orange -> Light Orange
        '#FFFF00': '#FFFF99',  // Yellow -> Light Yellow
        '#008000': '#66B266',  // Green -> Light Green
        '#0000FF': '#7F7FFF',  // Blue -> Light Blue
        '#4B0082': '#8A2BE2',  // Indigo -> Light Indigo (using Violet)
        '#8A2BE2': '#C4A2D1',  // Violet -> Light Violet
    };
    return secondaryColors[primary] || primary;
};

const tertiaryColor = (primary) => {
    const tertiaryColors = {
        '#FF0000': '#7F0000',  // Red -> Dark Red
        '#FFA500': '#FF6A00',  // Orange -> Dark Orange
        '#FFFF00': '#B2B200',  // Yellow -> Dark Yellow
        '#008000': '#006600',  // Green -> Dark Green
        '#0000FF': '#00007F',  // Blue -> Dark Blue
        '#4B0082': '#2E0064',  // Indigo -> Dark Indigo
        '#8A2BE2': '#4B0082',  // Violet -> Dark Violet
    };
    return tertiaryColors[primary] || primary;
};

// FUNCTION TO GET COMPLEMENTARY COLOR (Inverse of Primary, Secondary, Tertiary)
const getComplementaryColor = (color) => {
    const complementaryColors = {
        '#FF0000': '#00FFFF',  // Red -> Cyan
        '#00FF00': '#FF00FF',  // Green -> Magenta
        '#0000FF': '#FFFF00',  // Blue -> Yellow
        '#FF7F7F': '#007F7F',  // Light Red -> Dark Cyan
        '#7FFF7F': '#7F007F',  // Light Green -> Dark Magenta
        '#7F7FFF': '#7F7F00',  // Light Blue -> Dark Yellow
        '#7F0000': '#00FFFF',  // Dark Red -> Cyan
        '#007F00': '#FF00FF',  // Dark Green -> Magenta
        '#00007F': '#FFFF00',  // Dark Blue -> Yellow
    };
    return complementaryColors[color] || color;
};

// FUNCTION TO GENERATE COLORS FOR GRADIENTS, BORDERS, AND SHADOWS
const generateColorScheme = () => {
    // Define the elements that need distinct colors
    const elements = ['cabecera', 'menu', 'nav1', 'nav2', 'banner', 'cuerpo1', 'pie'];

    // Store used color combinations to avoid repetition
    let usedPrimaryColors = [];
    let usedSecondaryColors = [];
    let usedTertiaryColors = [];

    // Loop through each element and apply different primary, secondary, and tertiary colors
    elements.forEach(element => {
        let primary, secondary, tertiary, primaryContrary, secondaryContrary, tertiaryContrary;

        // Ensure no repetition of primary, secondary, and tertiary colors
        do {
            primary = primaryColor();
        } while (usedPrimaryColors.includes(primary));
        
        // Add the chosen primary color to the used list
        usedPrimaryColors.push(primary);

        // Generate corresponding secondary and tertiary colors
        secondary = secondaryColor(primary);
        tertiary = tertiaryColor(primary);

        // Ensure secondary and tertiary colors are not repeated
        do {
            secondary = secondaryColor(primary);
        } while (usedSecondaryColors.includes(secondary));
        usedSecondaryColors.push(secondary);

        do {
            tertiary = tertiaryColor(primary);
        } while (usedTertiaryColors.includes(tertiary));
        usedTertiaryColors.push(tertiary);

        // Set complementary colors for hover
        primaryContrary = tertiaryColor(primary);
        secondaryContrary = secondaryColor(primary);
        tertiaryContrary = primaryColor();

        // Set the gradient, border, and shadow styles for each element
        elementStyle.setProperty(`--gradient-${element}`, `linear-gradient(to bottom, ${primary}, ${secondary})`);
        elementStyle.setProperty(`--border-${element}`, secondary);
        elementStyle.setProperty(`--shadow-${element}`, `0 0 10px ${tertiary}, 0 0 20px ${tertiaryContrary}`);

        // Set complementary colors for hover effect (inverse)
        elementStyle.setProperty(`--hover-gradient-${element}`, `linear-gradient(to bottom, ${getComplementaryColor(primaryContrary)}, ${getComplementaryColor(secondaryContrary)})`);
        elementStyle.setProperty(`--hover-border-${element}`, getComplementaryColor(secondaryContrary));
        elementStyle.setProperty(`--hover-shadow-${element}`, `0 0 10px ${getComplementaryColor(tertiaryContrary)}, 0 0 20px ${getComplementaryColor(tertiaryContrary)}`);
    });
};

// CALL THE COLOR SCHEME GENERATOR ON PAGE LOAD
window.onload = generateColorScheme;
