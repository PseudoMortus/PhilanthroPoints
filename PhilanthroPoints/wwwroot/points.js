
window.pointsStore = {
  load: function(){
    const raw = localStorage.getItem('points');
    return raw ? JSON.parse(raw) : { value: 0, name: null };
  },
  save: function(obj){ localStorage.setItem('points', JSON.stringify(obj)); },
  clear: function(){ localStorage.removeItem('points'); }
};
