// Enhanced Birthday Buddy - Combines CSS Character Animation with Walking Movement
window.initBuddyWalk = function() {
  // Prevent multiple instances
  if (window.__globalBuddyActive) {
    console.log('Buddy already active');
    return;
  }
  
  const buddy = document.getElementById('live-buddy');
  if (!buddy) {
    console.log('Buddy element not found, retrying...');
    setTimeout(window.initBuddyWalk, 500);
    return;
  }
  
  // Check if already initialized
  if (buddy.dataset.initialized === 'true') {
    console.log('Buddy already initialized');
    return;
  }
  
  window.__globalBuddyActive = true;
  buddy.dataset.initialized = 'true';
  console.log('🎬 Starting enhanced birthday buddy - CSS + Walking animation');
  
  const inner = buddy.querySelector('.cupcake-inner');
  const cup = buddy.querySelector('.cupcake');
  
  // Preserve existing CSS animations while adding walking
  const currentClasses = buddy.className;
  
  // Add walking animation classes
  buddy.classList.add('floating-buddy');
  if (inner) inner.classList.add('walking');
  
  // Different behavior based on mode
  const isCheckoutMode = currentClasses.includes('celebration-mode');
  const isBounceMode = currentClasses.includes('bounce-mode');
  
  let walkDuration = 5000; // Default 5 seconds
  
  // Adjust timing based on mode
  if (isCheckoutMode) {
    walkDuration = 8000; // Longer celebration walk
    console.log('🎉 Celebration mode - extended walk time');
  } else if (isBounceMode) {
    walkDuration = 3000; // Shorter intro walk
    console.log('👋 Bounce mode - quick intro walk');
  }
  
  // After walk duration, settle into corner with CSS animations intact
  setTimeout(function() {
    console.log('🛑 Walk time up - settling into corner with CSS animations');
    
    // Remove walking animations but keep CSS character animations
    buddy.classList.remove('floating-buddy');
    if (inner) inner.classList.remove('walking');
    
    // Move to corner while preserving character animations
    buddy.style.position = 'fixed';
    buddy.style.top = '80px';
    buddy.style.right = '20px';
    buddy.style.left = 'auto';
    buddy.style.transform = 'scale(0.8)';
    buddy.style.zIndex = '9999';
    
    // Add gentle idle breathing that works with existing CSS
    buddy.classList.add('idle');
    
    console.log('✅ Birthday buddy settled in corner with character animations preserved');
    
  }, walkDuration);
  
  console.log(`⏱️ Timer set - buddy will walk for ${walkDuration/1000} seconds`);
};

// Enhanced reset function
window.resetBuddyWalk = function() {
  console.log('Resetting buddy walk state...');
  const buddy = document.getElementById('live-buddy');
  if (buddy) {
    buddy.dataset.initialized = 'false';
    buddy.classList.remove('floating-buddy', 'idle');
    buddy.style.position = '';
    buddy.style.top = '';
    buddy.style.right = '';
    buddy.style.left = '';
    buddy.style.transform = '';
    buddy.style.zIndex = '';
    
    const inner = buddy.querySelector('.cupcake-inner');
    if (inner) inner.classList.remove('walking');
  }
  window.__globalBuddyActive = false;
};

// Auto-reset on navigation for Blazor
window.addEventListener('beforeunload', window.resetBuddyWalk);
