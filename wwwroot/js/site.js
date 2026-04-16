window.jobBoard = {
  initBackToTop: function(btnId) {
    const btn = document.getElementById(btnId);
    if(!btn) return;
    window.addEventListener('scroll', function() {
      if (window.scrollY > 200) btn.classList.remove('d-none'); else btn.classList.add('d-none');
    });
    btn.addEventListener('click', function() {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  },
  initFadeIn: function() {
    const items = document.querySelectorAll('.animate-fade-in');
    const obs = new IntersectionObserver((entries) => {
              entries.forEach(e => {
         if (e.isIntersecting) {
           e.target.classList.add('visible');
         }
      });
    }, { threshold: 0.1 });
    items.forEach(it => obs.observe(it));
  },
  scrollToTop: function() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

