// Cart count update
function updateCartCount() {
  fetch('/Cart/GetCartCount')
    .then(r => r.json())
    .then(d => {
      const b = document.getElementById('cart-count');
      if (b) b.textContent = d.count || 0;
    }).catch(() => {});
}

// Toast notification
function showToast(msg, type = 'success') {
  const t = document.createElement('div');
  const bg = type === 'success' ? '#198754' : type === 'error' ? '#dc3545' : '#0d6efd';
  t.style.cssText = `position:fixed;bottom:80px;right:20px;background:${bg};color:#fff;padding:12px 20px;border-radius:8px;z-index:9999;font-size:13px;box-shadow:0 4px 12px rgba(0,0,0,.2);max-width:300px`;
  t.innerHTML = `<i class="bi bi-${type==='success'?'check-circle':'exclamation-circle'} me-2"></i>${msg}`;
  document.body.appendChild(t);
  setTimeout(() => t.style.opacity = '0', 2200);
  setTimeout(() => t.remove(), 2600);
}

// Auto dismiss alerts after 4 sec
document.addEventListener('DOMContentLoaded', () => {
  updateCartCount();
  setTimeout(() => {
    document.querySelectorAll('.alert').forEach(a => {
      try { new bootstrap.Alert(a).close(); } catch(e) {}
    });
  }, 4000);

  // Country / Language flag dropdown
  const savedFlag = localStorage.getItem('selectedFlag') || '/images/misc/flag-us.png';
  const savedLabel = localStorage.getItem('selectedLabel') || 'English | USD';
  const updateFlags = (flag, label) => {
    const selFlag = document.getElementById('selected-flag');
    const selLang = document.getElementById('selected-lang');
    const ftFlag  = document.getElementById('footer-flag');
    const ftLang  = document.getElementById('footer-lang');
    if (selFlag) selFlag.src = flag;
    if (selLang) selLang.textContent = label;
    if (ftFlag)  ftFlag.src = flag;
    if (ftLang)  ftLang.textContent = label.split(' | ')[0];
  };
  updateFlags(savedFlag, savedLabel);

  document.querySelectorAll('.lang-option').forEach(el => {
    el.addEventListener('click', e => {
      e.preventDefault();
      const flag  = el.dataset.flag;
      const label = el.dataset.label;
      localStorage.setItem('selectedFlag', flag);
      localStorage.setItem('selectedLabel', label);
      updateFlags(flag, label);
    });
  });
});
