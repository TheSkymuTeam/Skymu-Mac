// ------------------------------------------------------------------------------
// Global variables

// Arguments provided for initializeMap function
var gBingKey = null;
var gInitialZoomLevel = 0;
var gInitialMapType = "r";
var gIsPicking = false;
var gUserCanChooseLocation = false;
var gInitialLatitude = 0.0;
var gInitialLongitude = 0.0;
var gShowMapTypeSelector = false;
var gShowDashboard = false;
var gShowBreadcrumb = false;
var gLabelOverlay = 1; // labels invisible
var gMap = null;
var gGeoLocationProvider = null;

var gFractionalZoomLevel = -1.0;
var gChangingFractionalZoomLevel = false;
var gFractionalHeading = 999999.0;
var gChangingFractionalHeading = false;
var gMouseDownX = 0;
var gMouseDownY = 0;
var gMouseDownTime = 0;

var gLocatorPin = null;

// ------------------------------------------------------------------------------
// Event callbacks

function onMapTypeChanged() {
    
    var mapTypeId = gMap.getMapTypeId();
    console.log("New map type: " + mapTypeId);
    window.bingMapJSAPI.mapTypeDidChange(mapTypeId);
}

function onImageryChanged() {
    var mapTypeId = gMap.getMapTypeId();
    console.log("ImageryChanged: " + mapTypeId);
    //window.bingMapJSAPI.mapTypeDidChange(mapTypeId);
}



function onTileDownloadDidComplete (e) {
	console.log("onTileDownloadDidComplete function called");
    
    var mapOptions = gMap.getOptions();
    var labelOverlay = mapOptions.labelOverlay;
    
    if (labelOverlay != gLabelOverlay) {
        console.log("Options: " + mapOptions);
        console.log("Label Overlay: " + labelOverlay);
        gLabelOverlay = labelOverlay;
        window.bingMapJSAPI.labelOverlayDidChange(labelOverlay);
    }
    
	window.bingMapJSAPI.mapTileDownloadDidComplete();
}

function onViewChangeDidStart(e) {
    window.bingMapJSAPI.mapViewChangeDidStart();
}

function onViewChangeDidEnd() {
	var map = this.target; //reference to the Map object from which it came
	//...
	console.log('onViewChangeDidEnd called');
	if (gChangingFractionalZoomLevel)
		console.log("view change during fractional zoom");
	else {
		gFractionalZoomLevel = gMap.getZoom();
		console.log("View change New fractional zoom level "+ gFractionalZoomLevel);
	}
    
    if (gChangingFractionalHeading)
		console.log("view change during fractional zoom");
	else {
		gFractionalHeading = gMap.getHeading();
		console.log("View change New fractional zoom level "+ gFractionalZoomLevel);
	}
    window.bingMapJSAPI.mapViewChangeDidEnd();
}

function onKeyDown(e) {
    window.bingMapJSAPI.log("onKeyDown called");
    e.handled = window.bingMapJSAPI.mapKeyDown(e);
}

function onMouseWheel(e) {
	console.log("onMouseWheel called"); 
}

function onClick(e) {
	if (e.targetType == "map") {
		var point = new Microsoft.Maps.Point(e.getX(), e.getY());
		var loc = e.target.tryPixelToLocation(point);
		console.log("Click on " + loc.latitude + " , " + loc.longitude);
	}
}

function onMouseDown(e) {
	if (e.targetType == "map") {
		gMouseDownX = e.getX();
		gMouseDownY = e.getY();
		var d = new Date;
		gMouseDownTime = d.getTime();
	}
}

function onMouseUp(e) {
	if (e.targetType == "map") {
		var d = new Date;
		var mouseUpTime = d.getTime();
		var clickLength = mouseUpTime - gMouseDownTime;
		var mouseUpX = e.getX();
		var mouseUpY = e.getY();
		if (mouseUpX == gMouseDownX && mouseUpY == gMouseDownY && clickLength > 400 &&
			clickLength < 3000)
			handleLongClick(e);
	}
	gMouseDownTime = 0;
	gMouseDownX = 0;
	gMouseDownY = 0;
}

function handleLongClick(e) {
	if (e.targetType == "map") {
        if (gUserCanChooseLocation) {
            var point = new Microsoft.Maps.Point(e.getX(), e.getY());
            var loc = e.target.tryPixelToLocation(point);
            console.log("LONG CLICK on " + loc.latitude + " , " + loc.longitude);
            
            if (!gLocatorPin) {
                gLocatorPin = new Microsoft.Maps.Pushpin(loc);
                gMap.entities.push(gLocatorPin);
            }
            else {
                gLocatorPin.setLocation(loc);
            }
            
            window.bingMapJSAPI.userDidLongClickMap(loc.latitude, loc.longitude);
        }
	}
}

// ------------------------------------------------------------------------------
// Load the map.

// initializeMap is called by onLoad of HTML
function initializeMap(bingKey, initialZoomLevel, initialMapType, initialLabelOverlay, isPicking, userCanChooseLocation, showMapTypeSelector, showDashboard, showBreadcrumb, latitude, longitude) {
	gBingKey = bingKey;
	gInitialZoomLevel = initialZoomLevel;
    gInitialMapType = initialMapType;
    gLabelOverlay = initialLabelOverlay;
	gIsPicking = isPicking;
    gUserCanChooseLocation = userCanChooseLocation;
	gInitialLatitude = latitude;
	gInitialLongitude = longitude;
	gShowMapTypeSelector = showMapTypeSelector;
    gShowDashboard = showDashboard;
    gShowBreadcrumb = showBreadcrumb;
	console.log("Initialize zoom=" + gInitialZoomLevel + " picking=" + gIsPicking +  " userCanChooseLocation=" + gUserCanChooseLocation + " latitude=" + gInitialLatitude + " longitude=" + gInitialLongitude);
	
    if (gShowMapTypeSelector || gShowDashboard ||  showBreadcrumb) {
        console.log("Load Overlays Style");
        Microsoft.Maps.loadModule('Microsoft.Maps.Overlays.Style', {callback:loadMap});
    }
    else
        loadMap();
}

function loadMap() {
	console.log("Start loading map");
	var theLocation = new Microsoft.Maps.Location(gInitialLatitude, gInitialLongitude);
	var mapOptions = {
		credentials: gBingKey,
		center: theLocation,
		zoom: gInitialZoomLevel,
		mapTypeId: gInitialMapType,
		enableSearchLogo: false,
		enableClickableLogo: false,
        showScalebar: true,
		showMapTypeSelector: gShowMapTypeSelector,
		showDashboard: gShowDashboard,
        showBreadcrumb: gShowBreadcrumb,
        labelOverlay: gLabelOverlay,
		// Set disablePanning to true to turn panning off, and see touchend line below.
		disablePanning: false,
		
		customizeOverlays : true,
	};
    
	mapDiv = document.getElementById("mapDiv");
	gMap = new Microsoft.Maps.Map(mapDiv, mapOptions);
	
	Microsoft.Maps.Events.addHandler(gMap, "tiledownloadcomplete", onTileDownloadDidComplete);
    Microsoft.Maps.Events.addHandler(gMap, "viewchangestart", onViewChangeDidStart);
    Microsoft.Maps.Events.addHandler(gMap, "viewchangeend", onViewChangeDidEnd);
    Microsoft.Maps.Events.addHandler(gMap, "keydown", onKeyDown);
    Microsoft.Maps.Events.addHandler(gMap, "click", onClick);
	Microsoft.Maps.Events.addHandler(gMap, "mousedown", onMouseDown);
	Microsoft.Maps.Events.addHandler(gMap, "mouseup", onMouseUp);
    Microsoft.Maps.Events.addHandler(gMap, "maptypechanged", onMapTypeChanged);
    Microsoft.Maps.Events.addHandler(gMap, "imagerychanged", onImageryChanged);
    
    
	console.log("Finish loading map");
    window.bingMapJSAPI.mapInitializationDidEnd();
}

// ------------------------------------------------------------------------------
// Generic functions

function removeLocatorPin() {
    if (gLocatorPin) {
        gMap.entities.remove(gLocatorPin);
        gLocatorPin = null;
    }
    window.bingMapJSAPI.log("removeLocatorPin called");
}

function setCenterCoordinateAndZoomIn(latitude, longitude, zoomLevel, animated) {
	var latlong = new Microsoft.Maps.Location(latitude, longitude);
	var mapOptions = {
		center:latlong,
		zoom:zoomLevel,
        animate:animated
	};
	gMap.setView(mapOptions);
}

function setCenterCoordinate(latitude, longitude, animated) {
	var latlong = new Microsoft.Maps.Location(latitude, longitude);
	var mapOptions = {
        center:latlong,
        animate:animated
	};
	gMap.setView(mapOptions);
}

function setZoomLevel(zoomLevel, animated) {
    var mapOptions = {
        zoom:zoomLevel,
        animate:animated
	};
	gMap.setView(mapOptions);
}

function makeAllPinsVisible()
{
	var entities = gMap.entities;
	var length = entities.getLength();
	var locations = [];
	for(var i = 0; i < length; i++){
		var theEntity = entities.get(i);
		var theLocation = theEntity.getLocation();
		locations[i] = theLocation;
	}
	var viewRect = Microsoft.Maps.LocationRect.fromLocations(locations);
	gMap.setView({bounds: viewRect});
}

function displayPinAtLatLong(pinText, pinWidth, pinHeight, iconPath, latitude, longitude, anchorX, anchorY) {
	var theLocation = new Microsoft.Maps.Location(latitude, longitude);
	var pinAnchor = new Microsoft.Maps.Point(anchorX, anchorY);
	displayPinAtLocation(pinText, pinWidth, pinHeight, pinAnchor, theLocation, iconPath);
	return "OK";
}

function displayPinAtLocation(pinText, pinWidth, pinHeight, pinAnchor, theLocation, iconPath) {
	var pin = findPinWithText(pinText);
	if(pin == null){
		var pinTextOffset = new Microsoft.Maps.Point(0, pinHeight + 100);
		
		var htmlx = '<div><img src="file://' + iconPath + '" width="' + pinWidth + '" height="' + pinHeight + '"></div>';
		var pinOptions = { draggable: false, text: pinText, width: pinWidth, height: pinHeight, anchor: pinAnchor, htmlContent : htmlx, textOffset: pinTextOffset};
		
		var pin = new Microsoft.Maps.Pushpin(theLocation, pinOptions);
		gMap.entities.push(pin);
	}
	else {
		pin.setLocation(theLocation);
	}
}

function findPinWithText(theText) {
	var pinWithText = null;
	var entities = gMap.entities;
	var length = entities.getLength();
	
	for(var i = 0; i < length; i++){
		var pin = entities.get(i);
		var pinText = pin.getText();
		if(pinText == theText){
			pinWithText = pin;
			break;
		}
	}
	return pinWithText;
}

// ------------------------------------------------------------------------------
// Pinch zoom and rotation support

function setFractionalZoomLevel(zoomLevel) {
	gChangingFractionalZoomLevel = true;
	gFractionalZoomLevel = zoomLevel;
	var integralZoomLevel = Math.round(gFractionalZoomLevel);
	var mapOptions = {
		zoom: integralZoomLevel
	};
	gMap.setView(mapOptions);
	gChangingFractionalZoomLevel = false;
    console.log("setFractionalZoomLevel called with zoom" + zoomLevel);
    window.bingMapJSAPI.log("setFractionalZoomLevel called with zoom" + zoomLevel);
}

function fractionalZoomLevel() {
	if (gFractionalZoomLevel < 0) {
        var integralZoomLevel = gMap.getZoom();
		gFractionalZoomLevel = integralZoomLevel;
	}
	return gFractionalZoomLevel;
}

function canRotate() {
    var mapTypeId = gMap.getMapTypeId()
    return ((mapTypeId == "a" || mapTypeId == "be") && gFractionalZoomLevel > 7)
}

function setFractionalHeading(newHeading) {
    gChangingFractionalHeading = true;
    gFractionalHeading = newHeading;
    var integralHeading = Math.round(gFractionalHeading);
    var mapOptions = {
        heading: integralHeading
	};
    gMap.setView(mapOptions);
    gChangingFractionalHeading = false;
}

function fractionalHeading() {
    if (gFractionalHeading > 9999.0) {
        gFractionalHeading = gMap.getHeading();
    }
    return gFractionalHeading;
}





