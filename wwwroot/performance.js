// Performance optimization script for Birthday connections
// Lazy load and optimize page interactions

(function() {
    'use strict';

    // Defer non-critical animations until after page load
    window.addEventListener('load', function() {
        // Enable floating decorations after page loads
        setTimeout(function() {
            const decorations = document.querySelectorAll('.floating-decoration');
            decorations.forEach(decoration => {
                decoration.classList.add('loaded');
            });
        }, 500);

        // Preload next likely pages based on user interaction
        preloadLikelyPages();
    });

    // Preload likely next pages for faster navigation
    function preloadLikelyPages() {
        const currentPath = window.location.pathname;
        const preloadTargets = [];

        // Determine likely next pages based on current page
        if (currentPath === '/' || currentPath.endsWith('/Home')) {
            preloadTargets.push('/Demographics', '/Cards', '/Treats', '/Gifts');
        } else if (currentPath.includes('Demographics')) {
            preloadTargets.push('/Cards');
        } else if (currentPath.includes('Cards')) {
            preloadTargets.push('/Treats');
        } else if (currentPath.includes('Treats')) {
            preloadTargets.push('/Gifts');
        } else if (currentPath.includes('Gifts')) {
            preloadTargets.push('/Parent/Cart');
        }

        // Create hidden preload links
        preloadTargets.forEach(function(target) {
            const link = document.createElement('link');
            link.rel = 'prefetch';
            link.href = target;
            document.head.appendChild(link);
        });
    }

    // Optimize button clicks with debouncing
    const debouncedClicks = new Map();
    function debounceClick(element, handler, delay = 200) {
        element.addEventListener('click', function(e) {
            const key = element.textContent + element.className;
            if (debouncedClicks.has(key)) {
                clearTimeout(debouncedClicks.get(key));
            }
            debouncedClicks.set(key, setTimeout(() => {
                handler.call(this, e);
                debouncedClicks.delete(key);
            }, delay));
        });
    }

    // Lazy load heavy content when it comes into view
    if ('IntersectionObserver' in window) {
        const lazyLoadObserver = new IntersectionObserver(function(entries) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    const element = entry.target;
                    
                    // Enable heavy animations when element is visible
                    if (element.classList.contains('heavy-animation')) {
                        element.classList.add('animation-enabled');
                        lazyLoadObserver.unobserve(element);
                    }
                }
            });
        }, {
            rootMargin: '50px'
        });

        // Observe elements when DOM is ready
        document.addEventListener('DOMContentLoaded', function() {
            const heavyElements = document.querySelectorAll('.heavy-animation');
            heavyElements.forEach(element => {
                lazyLoadObserver.observe(element);
            });
        });
    }

    // Optimize Blazor reconnection
    if (window.Blazor) {
        Blazor.defaultReconnectionHandler.onConnectionDown = function(options, error) {
            console.log('Connection lost, attempting optimized reconnection...');
        };
    }

    // Performance monitoring (dev mode only)
    if (window.location.hostname === 'localhost') {
        window.addEventListener('load', function() {
            setTimeout(function() {
                if (performance.getEntriesByType) {
                    const loadTime = performance.getEntriesByType('navigation')[0]?.loadEventEnd || 0;
                    console.log(`Birthday connections loaded in ${Math.round(loadTime)}ms`);
                    
                    if (loadTime > 3000) {
                        console.warn('Slow loading detected. Consider optimizing resources.');
                    }
                }
            }, 100);
        });
    }

})();