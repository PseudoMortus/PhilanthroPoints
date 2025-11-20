// Static Birthday Buddy - All animations removed
window.initBuddyWalk = function() {
  // Prevent multiple instances
  if (window.__globalBuddyActive) {
    console.log('Buddy already active');
    return;
  }
  
  const buddy = document.getElementById('live-buddy');
  if (!buddy) {
    console.log('Buddy element not found');
    return;
  }
  
  // Check if already initialized
  if (buddy.dataset.initialized === 'true') {
    console.log('Buddy already initialized');
    return;
  }
  
  window.__globalBuddyActive = true;
  buddy.dataset.initialized = 'true';
<<<<<<< HEAD
  console.log('🎬 Static birthday buddy positioned');
=======
  console.log('Static birthday buddy positioned');
>>>>>>> Mike's-Commits
  
  // Position buddy in corner - no animations
  buddy.style.position = 'fixed';
  buddy.style.top = '80px';
  buddy.style.right = '20px';
  buddy.style.left = 'auto';
  buddy.style.transform = 'scale(0.8)';
  buddy.style.zIndex = '9999';
  
<<<<<<< HEAD
  console.log('✅ Birthday buddy positioned statically in corner');
=======
  console.log('Birthday buddy positioned statically in corner');
>>>>>>> Mike's-Commits
};

// Reset function
window.resetBuddyWalk = function() {
  console.log('Resetting buddy state...');
  const buddy = document.getElementById('live-buddy');
  if (buddy) {
    buddy.dataset.initialized = 'false';
    buddy.style.position = '';
    buddy.style.top = '';
    buddy.style.right = '';
    buddy.style.left = '';
    buddy.style.transform = '';
    buddy.style.zIndex = '';
  }
  window.__globalBuddyActive = false;
};

// Auto-reset on navigation for Blazor
window.addEventListener('beforeunload', window.resetBuddyWalk);
