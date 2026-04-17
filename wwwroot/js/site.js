window.jobBoard = (function() {
  // Private helpers
  function safeQuery(selector, root = document) { return root.querySelector(selector); }

  return {
    initBackToTop: function(btnId = 'backToTopBtn') {
      const btn = document.getElementById(btnId);
      if (!btn) return;

      window.addEventListener('scroll', function() {
        if (window.scrollY > 200) btn.classList.remove('d-none'); else btn.classList.add('d-none');
      });

      btn.addEventListener('click', function() {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });
    },

    initFadeIn: function() {
      const items = document.querySelectorAll('.animate-fade-in');
      if (!items || items.length === 0) return;

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
    },

    redirectHome: function() {
      window.location.href = '/';
    },

    // Perform login via browser fetch so Set-Cookie can be stored by the browser
    login: async function(url, model) {
      const res = await fetch(url, {
        method: 'POST',
        credentials: 'same-origin',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(model)
      });
      const text = await res.text();
      return { status: res.status, text };
    },

    logout: async function(url) {
      console.log('[jobBoard] logout: sending POST to', url);
      const res = await fetch(url, { method: 'POST', credentials: 'same-origin' });
      const text = await res.text();
      console.log('[jobBoard] logout: response', res.status, text);
      return { status: res.status, text };
    },

    // Simple hero slider: switches hero text and active dot
    initHeroSlider: function(heroSelector) {
      try {
        const hero = document.querySelector(heroSelector);
        if (!hero) return;

        const dots = hero.querySelectorAll('.slider-dot');
        const navBtns = hero.querySelectorAll('.slider-nav-btn');

        let current = 0;
        const slideCount = dots.length || 1;

        function setActive(index) {
          current = (index + slideCount) % slideCount;
          dots.forEach((d, i) => d.classList.toggle('active', i === current));

          const title = hero.querySelector('h1');
          const lead = hero.querySelector('p.lead');
          if (title && title.dataset && title.dataset['slide' + current]) {
            title.textContent = title.dataset['slide' + current];
          }
          if (lead && lead.dataset && lead.dataset['slide' + current]) {
            lead.textContent = lead.dataset['slide' + current];
          }
        }

        dots.forEach((d, i) => d.addEventListener('click', () => setActive(i)));
        if (navBtns && navBtns.length > 0) {
          // assume first is prev, second is next when present
          if (navBtns[0]) navBtns[0].addEventListener('click', () => setActive(current - 1));
          if (navBtns[1]) navBtns[1].addEventListener('click', () => setActive(current + 1));
        }

        setActive(0);
      } catch (e) {
        console.error('initHeroSlider error', e);
      }
    }
  };
})();
