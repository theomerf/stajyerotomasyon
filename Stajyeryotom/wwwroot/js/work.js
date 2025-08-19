function initWorkCircles() {
    document.querySelectorAll('.work-circle').forEach(circle => {
        const progress = circle.dataset.progress;
        const progressRing = circle.querySelector('.work-progress-fill');
        const radius = progressRing.r.baseVal.value;
        const circumference = radius * 2 * Math.PI;

        progressRing.style.strokeDasharray = `${circumference} ${circumference}`;
        progressRing.style.strokeDashoffset = circumference;

        setTimeout(() => {
            const offset = circumference - (progress / 100) * circumference;
            progressRing.style.strokeDashoffset = offset;
        }, 200);
    });
}