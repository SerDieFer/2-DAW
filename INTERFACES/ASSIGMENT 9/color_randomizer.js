let elementStyle = document.documentElement.style;

// FUNCTION TO GENERATE RANDOM HUE (BASE COLOR)
let randomHue = () => Math.floor(Math.random() * 360);

// FUNCTION TO GENERATE PRIMARY, LIGHTER (SECONDARY), AND DARKER (TERTIARY) COLORS
let generateColorShades = (hue) => {
    let primary = `hsl(${hue}, 80%, 50%)`;      // BASE COLOR
    let secondary = `hsl(${hue}, 70%, 70%)`;    // LIGHTER SHADE
    let tertiary = `hsl(${hue}, 90%, 30%)`;     // DARKER SHADE
    return { primary, secondary, tertiary };
};

// FUNCTION TO CALCULATE COMPLEMENTARY COLOR
let getComplementaryColor = (hue) => (hue + 180) % 360; // 180-DEGREE HUE SHIFT FOR COMPLEMENTARY

// FUNCTION TO APPLY GENERATED COLOR SCHEME
const generateColorScheme = () => {
    let elements = ['cabecera', 'menu', 'nav1', 'nav2', 'banner', 'cuerpo1', 'pie'];
    let usedHues = new Set();

    elements.forEach(element => {
        let hue;
        
        // GENERATE A UNIQUE HUE
        do {
            hue = randomHue();
        } while (usedHues.has(hue));
        usedHues.add(hue);

        // GENERATE PRIMARY, SECONDARY, TERTIARY SHADES
        let { primary, secondary, tertiary } = generateColorShades(hue);

        // GENERATE COMPLEMENTARY COLORS FOR HOVER
        let complementaryHue = getComplementaryColor(hue);
        let { primary: primaryComp, secondary: secondaryComp, tertiary: tertiaryComp } = generateColorShades(complementaryHue);

        // SET STYLES DYNAMICALLY
        elementStyle.setProperty(`--gradient-${element}`, `linear-gradient(to bottom, ${primary}, ${secondary})`);
        elementStyle.setProperty(`--border-${element}`, secondary);
        elementStyle.setProperty(`--shadow-${element}`, `0 0 10px ${tertiary}, 0 0 20px ${tertiary}`);

        // HOVER STATES WITH COMPLEMENTARY COLORS
        elementStyle.setProperty(`--hover-gradient-${element}`, `linear-gradient(to bottom, ${primaryComp}, ${secondaryComp})`);
        elementStyle.setProperty(`--hover-border-${element}`, secondaryComp);
        elementStyle.setProperty(`--hover-shadow-${element}`, `0 0 10px ${tertiaryComp}, 0 0 20px ${tertiaryComp}`);
    });
};

// CALL THE COLOR SCHEME GENERATOR ON PAGE LOAD
window.onload = generateColorScheme;
