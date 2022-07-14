$(document).ready(function(){
	//INITIALIZE
	var video = $('#myVideo');
	
	//remove default control when JS loaded
	video[0].removeAttribute("controls");
	$('.control').show().css({'opacity':0});
	$('.loading').fadeIn(500);
	$('.caption').fadeIn(500);
	
	//disable link callback
	$('.mainControl a').on('click', function() {
		return false;
	});
	
	//before everything get started
	video.on('loadedmetadata', function() {
		
		//hide video caption
		$('.caption').animate({'opacity':0},300);
			
		//set video properties
		updateVolume(0, 0.75);
			
		//start to get video buffering data 
		setTimeout(startBuffer, 150);
			
		//bind video events
		$('.videoContainer').append('<div id="init"></div>')
		.on('click', function() {
			$('#init').remove();
			$('.btnPlay').addClass('paused');
			$(this).unbind('click');
			video[0].play();
		})
		.hover(function() {
			$('.control').stop().animate({'opacity':1}, 500);
			$('.caption').stop().animate({'opacity':1}, 500);
		}, function() {
			if(!timeDrag){
				$('.control').stop().animate({'opacity':0}, 500);
				$('.caption').stop().animate({'opacity':0}, 500);
			}
		});
		$('#init').fadeIn(200);
	});
	
	//display video buffering bar
	var startBuffer = function() {
		var currentBuffer = video[0].buffered.end(0);
		var maxduration = video[0].duration;
		var perc = 100 * currentBuffer / maxduration;
		$('.bufferBar').css('width',perc+'%');
			
		if(currentBuffer < maxduration) {
			setTimeout(startBuffer, 500);
		}
	};	
	
	//display current video play time
	video.on('timeupdate', function() {
		var currentPos = video[0].currentTime;
		var maxduration = video[0].duration;
		var perc = 100 * currentPos / maxduration;
		$('.timeBar').css('width',perc+'%');	
	});

	//CONTROLS EVENTS
	//video screen and play button clicked
	video.on('click', function() { playpause(); } );
	$('.btnPlay').on('click', function() { playpause(); } );
	var playpause = function() {
		//automatic reset playbackrate
		video[0].playbackRate = 1;

		if(video[0].paused || video[0].ended || !$('.btnPlay').hasClass('paused')) {
			$('.btnPlay').addClass('paused');
			video[0].play();
		}
		else {
			$('.btnPlay').removeClass('paused');
			video[0].pause();
		}
	};
	
	//fast forward button clicked
	//unfortunately Firefox doesn't support video playbackrate yet!
	//and Chrome doesn't support negative value for playbackrate yet!
	//currently only works in Safari
	$('.btnBck').on('click', function() { fastfowrd(this, -2); });
	$('.btnFwd').on('click', function() { fastfowrd(this, 2); });
	var fastfowrd = function(obj, spd) {
		//user have to click on play button to return to normal speed 
		$('.btnPlay').removeClass('paused');
		
		video[0].playbackRate = spd;
		video[0].play();
	};
	
	//stop or end button clicked
	$('.btnStop').on('click', function() { stopvideo(this); });
	$('.btnEnd').on('click', function() { stopvideo(this, $('.progress').width()); });
	var stopvideo = function(obj, width) {
		//update progress bar and currenttime
		if(!width) {
			width = 0;
		}
		var pos = $('.progress').offset().left + width;
		updatebar(pos);
		$('.btnPlay').removeClass('paused');
		video[0].pause();
	};
	
	//sound button clicked
	$('.sound').click(function() {
		video[0].muted = !video[0].muted;
		$(this).toggleClass('muted');
		$('.volumeBar').toggleClass('volumeoff');
	});
	
	//VIDEO EVENTS
	//video canplay event
	video.on('canplay', function() {
		$('.loading').fadeOut(100);
	});
	
	//video canplaythrough event
	//solve Chrome cache issue
	var completeloaded = false;
	video.on('canplaythrough', function() {
		completeloaded = true;
	});
	
	//video ended event
	video.on('ended', function() {
		$('.btnPlay').removeClass('paused');
		video[0].pause();
	});
	
	//video seeking event
	video.on('seeking', function() {
		//if video fully loaded, ignore loading screen
		if(!completeloaded) {
			$('.loading').fadeIn(200);
		}	
	});
	
	//video seeked event
	video.on('seeked', function() { });
	
	//video waiting for more data event
	video.on('waiting', function() {
		$('.loading').fadeIn(200);
	});
	
	//VIDEO PROGRESS BAR
	//when video timebar clicked
	var timeDrag = false;	/* check for drag event */
	$('.progress').mousedown(function(e) {
		timeDrag = true;
		updatebar(e.pageX);
	});
	$(document).mouseup(function(e) {
		if(timeDrag) {
			timeDrag = false;
			updatebar(e.pageX);
		}
	});
	$(document).mousemove(function(e) {
		if(timeDrag) {
			updatebar(e.pageX);
		}
	});
	var updatebar = function(x) {
		var progress = $('.progress');
		
		//calculate drag position
		//and update video currenttime
		//as well as progress bar
		var maxduration = video[0].duration;
		var position = x - progress.offset().left;
		var percentage = 100 * position / progress.width();
		if(percentage > 100) {
			percentage = 100;
		}
		if(percentage < 0) {
			percentage = 0;
		}
		$('.timeBar').css('width',percentage+'%');	
		video[0].currentTime = maxduration * percentage / 100;
	};

	//VOLUME BAR
	//volume bar event
	$('.volumeCover').on('mousedown', function(e) {
		updateVolume(e.pageX);
	});
	var updateVolume = function(x, vol) {
		var volume = $('.volume');
		var percentage;
		//if only volume have specificed
		//then direct update volume
		if(vol) {
			percentage = vol * 100;
		}
		else {
			var position = x - volume.offset().left;
			percentage = 100 * position / volume.width();	
		}
		//ceil to 25 mutiplier
		percentage = Math.ceil(percentage/25)*25;
		
		$('.volumeBar').css('width',percentage+'%');	
		video[0].volume = percentage / 100;
	};

});