// Tailwind (CDN) tema ayarlari: Stitch'in urettigi renk, font ve olcu degerleri.
// Bu dosya, _ChatLayout.cshtml icinde Tailwind CDN script'inden SONRA yuklenir.
tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            "colors": {
                "tertiary": "#78555e",
                "background": "#faf9f6",
                "on-background": "#1a1c1a",
                "surface-variant": "#e3e2e0",
                "tertiary-fixed-dim": "#e7bbc6",
                "on-secondary-container": "#626374",
                "outline": "#7a757f",
                "on-tertiary-fixed": "#2d141c",
                "on-surface-variant": "#49454f",
                "inverse-primary": "#d2bcfb",
                "on-secondary-fixed-variant": "#444655",
                "tertiary-fixed": "#ffd9e2",
                "primary-fixed": "#ebddff",
                "on-error": "#ffffff",
                "surface-container-highest": "#e3e2e0",
                "primary": "#67558c",
                "on-secondary-fixed": "#191b29",
                "surface-container-low": "#f4f3f1",
                "surface-dim": "#dbdad7",
                "on-primary-container": "#443267",
                "on-surface": "#1a1c1a",
                "surface-container-lowest": "#ffffff",
                "secondary-fixed": "#e1e1f5",
                "surface-container-high": "#e9e8e5",
                "surface-bright": "#faf9f6",
                "primary-container": "#b19cd9",
                "on-tertiary": "#ffffff",
                "tertiary-container": "#c59ba6",
                "on-primary-fixed-variant": "#4f3d73",
                "surface-tint": "#67558c",
                "inverse-surface": "#2f312f",
                "primary-fixed-dim": "#d2bcfb",
                "on-primary": "#ffffff",
                "outline-variant": "#cbc4d0",
                "on-tertiary-container": "#51333c",
                "secondary-fixed-dim": "#c5c5d8",
                "on-secondary": "#ffffff",
                "surface": "#faf9f6",
                "surface-container": "#efeeeb",
                "on-error-container": "#93000a",
                "on-primary-fixed": "#220f44",
                "inverse-on-surface": "#f2f1ee",
                "on-tertiary-fixed-variant": "#5e3e47",
                "secondary": "#5c5d6e",
                "error-container": "#ffdad6",
                "secondary-container": "#e1e1f5",
                "error": "#ba1a1a"
            },
            "borderRadius": {
                "DEFAULT": "0.25rem",
                "lg": "0.5rem",
                "xl": "0.75rem",
                "full": "9999px"
            },
            "spacing": {
                "gutter": "24px",
                "stack-md": "24px",
                "stack-sm": "12px",
                "container-max": "1200px",
                "margin-mobile": "16px",
                "stack-lg": "48px",
                "unit": "8px"
            },
            "fontFamily": {
                "headline-md": ["Inter"],
                "code-snippet": ["JetBrains Mono"],
                "display-lg-mobile": ["Inter"],
                "display-lg": ["Inter"],
                "label-sm": ["Inter"],
                "body-lg": ["Inter"],
                "body-md": ["Inter"]
            },
            "fontSize": {
                "headline-md": ["24px", { "lineHeight": "32px", "letterSpacing": "-0.01em", "fontWeight": "600" }],
                "code-snippet": ["14px", { "lineHeight": "20px", "fontWeight": "400" }],
                "display-lg-mobile": ["32px", { "lineHeight": "40px", "letterSpacing": "-0.02em", "fontWeight": "700" }],
                "display-lg": ["48px", { "lineHeight": "56px", "letterSpacing": "-0.02em", "fontWeight": "700" }],
                "label-sm": ["13px", { "lineHeight": "18px", "letterSpacing": "0.01em", "fontWeight": "500" }],
                "body-lg": ["18px", { "lineHeight": "28px", "fontWeight": "400" }],
                "body-md": ["16px", { "lineHeight": "24px", "fontWeight": "400" }]
            }
        }
    }
};
