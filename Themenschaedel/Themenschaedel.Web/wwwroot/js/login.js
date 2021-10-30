

window.loginAnimation = () => {
    console.log('test');
    setTimeout(function() {
        $('.logoContainer').transition({scale: 1}, 700, 'ease');
        setTimeout(function() {
            $('.logoContainer .logo').addClass('loadIn');
            setTimeout(function() {
                $('.logoContainer .text').addClass('loadIn');
                setTimeout(function() {
                    $('.acceptContainer').transition({height: '475px'});
                    setTimeout(function() {
                        $('.acceptContainer').addClass('loadIn');
                        setTimeout(function() {
                            $('.formDiv, form h1').addClass('loadIn');
                        }, 500)
                    }, 500)
                }, 500)
            }, 500)
        }, 1000)
    }, 10)
}