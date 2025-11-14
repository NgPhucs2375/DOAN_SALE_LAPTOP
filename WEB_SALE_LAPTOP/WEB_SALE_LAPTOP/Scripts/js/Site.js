// Chờ cho toàn bộ nội dung trang được tải xong
window.addEventListener('load', function () {

    console.log("site.js (v2 - Tiến hóa) đã tải!");

    const logoButton = document.querySelector('.logo-container');
    const body = document.body;
    const sidebarStateKey = 'sidebarCollapsedState'; // Chìa khóa để lưu

    if (logoButton) {

        // ===== 1. LOGIC ẨN/HIỆN SIDEBAR (Sửa lỗi 4) =====

        // A. KIỂM TRA TRẠNG THÁI KHI TẢI TRANG
        // Kiểm tra xem trình duyệt có "nhớ" trạng thái thu nhỏ từ lần trước không
        if (localStorage.getItem(sidebarStateKey) === 'true') {
            body.classList.add('sidebar-collapsed');
        }

        // B. HÀNH ĐỘNG KHI CLICK
        logoButton.addEventListener('click', function () {
            body.classList.toggle('sidebar-collapsed');

            // LƯU TRẠNG THÁI MỚI vào bộ nhớ trình duyệt
            if (body.classList.contains('sidebar-collapsed')) {
                localStorage.setItem(sidebarStateKey, 'true');
            } else {
                localStorage.setItem(sidebarStateKey, 'false');
            }
        });
    } else {
        console.error("Lỗi: Không tìm thấy '.logo-container'");
    }

    // ===== 2. LOGIC ACCORDION CHO SIDEBAR (Sửa lỗi 2 - Giữ nguyên) =====
    const filterGroups = document.querySelectorAll('.filter-group-title');

    filterGroups.forEach(groupTitle => {
        groupTitle.addEventListener('click', function () {
            const parentGroup = this.parentElement;
            if (!parentGroup) return;
            const content = parentGroup.querySelector('.filter-group-content');
            if (!content) return;

            if (parentGroup.classList.contains('active')) {
                parentGroup.classList.remove('active');
                content.style.maxHeight = null;
            } else {
                parentGroup.classList.add('active');
                content.style.maxHeight = content.scrollHeight + 'px';
            }
        });
    });

    // Tự động mở mục đầu tiên có class 'active'
    const activeGroup = document.querySelector('.sidebar .filter-group.active');
    if (activeGroup) {
        const content = activeGroup.querySelector('.filter-group-content');
        if (content) {
            setTimeout(() => {
                content.style.maxHeight = content.scrollHeight + 'px';
            }, 100);
        }
    }
});