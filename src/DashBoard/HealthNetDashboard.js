function HealthNetDashboard(dashboardId, env) {
    var dashboardElement = document.getElementById(dashboardId);
    var environments = env;

    this.checkHealth = function() {
        var endpointElements = dashboardElement.getElementsByClassName("HealthNetEndpoint");
        for (var i = 0; i < endpointElements.length; i++) {
            endpointElements[i].performHealthCheck();
        }
    };

    var updateEndpointHealthStatus = function(endpointElement, healthCheckResult){
        endpointElement.className = "HealthNetEndpoint " + healthCheckResult.health;

        var popupId = healthCheckResult.host + "_RAW";
        var showRawResultButton = document.createElement("a");
        showRawResultButton.className = "HealthNetShowRaw";
        showRawResultButton.href = "#" + popupId;
        showRawResultButton.innerText = "{...}";
        endpointElement.appendChild(showRawResultButton);

        for (var i = 0; i < healthCheckResult.systemStates.length; i++){
            var systemState = healthCheckResult.systemStates[i];

            var systemStateElement = document.createElement("div");
            systemStateElement.innerText = systemState.systemName;
            systemStateElement.className = systemState.health;
            endpointElement.appendChild(systemStateElement);
        }

        var systemStateRawElement = document.createElement("div");
        systemStateRawElement.id = popupId;
        systemStateRawElement.innerText = JSON.stringify(healthCheckResult, null, '    ');
        systemStateRawElement.className = "HealthNetResultRaw";

        var rawContent = document.createElement("div");
        rawContent.className = "HealthNetRawContent";
        rawContent.innerText = JSON.stringify(healthCheckResult, null, '    ');

        var rawPopup = document.createElement("div");
        rawPopup.className = "HealthNetRawPopup";

        var closeRawPopup = document.createElement("a");
        closeRawPopup.className = "HealthNetCloseRaw";
        closeRawPopup.innerText = "x";
        closeRawPopup.href = "#";

        rawPopup.appendChild(closeRawPopup);
        rawPopup.appendChild(rawContent);
        systemStateRawElement.appendChild(rawPopup);
        endpointElement.parentElement.parentElement.appendChild(systemStateRawElement);
    };

    var setupHealthcheckCall = function(endpoint, endpointElement){
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function() {
            if (this.readyState == 4 && this.status == 200) {
                var healthResult = JSON.parse(this.responseText);
                updateEndpointHealthStatus(endpointElement, healthResult);
            }
        };
        xmlHttp.onerror = function() {
            endpointElement.className = "HealthNetEndpoint Critical";
        };
        endpointElement.performHealthCheck = function(){
            xmlHttp.open("GET", endpoint.endpointUrl, true);
            xmlHttp.send();
        };
    };
    var buildEndpointElement = function(endpoint) {
        var endpointElement = document.createElement("div");
        endpointElement.className = "HealthNetEndpoint HealthNetChecking";
        var endpointName = document.createElement("h1");
        endpointName.innerText = endpoint.endpointName;
        endpointElement.appendChild(endpointName);
        setupHealthcheckCall(endpoint, endpointElement);
        return endpointElement;
    };
    var buildEnvironmentElement = function (env) {
        var environmentDashboard = document.createElement("div");
        environmentDashboard.className = "HealthNetEnvironment";

        var environmentName = document.createElement("h1");
        environmentName.innerText = env.environmentName;

        environmentDashboard.appendChild(environmentName);

        for (var i = 0; i < env.environmentEndpoints.length; i++){
            environmentDashboard.appendChild(buildEndpointElement(env.environmentEndpoints[i]))
        }

        return environmentDashboard;
    };

    var setupDashboard = function() {
        dashboardElement.className = "HealthNetDashBoard";

        for (i = 0; i < environments.length; i++) {
            dashboardElement.appendChild(buildEnvironmentElement(environments[i]));
        }
    };

    setupDashboard();
}

HealthNetDashboard.createEnvironment = function(name, endpoints) {
    return { environmentName: name, environmentEndpoints: endpoints };
};
/*
function createHealthCheckEndpoint (url, name) {
    return { endPointUrl: url, endpointName: name };
}
/*

function showSystemsHealth(environmentNode, checks) {
    for (i = 0; i < checks.length; i++) {
        var xmlHttp = new XMLHttpRequest();

        var healthCheckNode = document.createElement("div");
        healthCheckNode.className = "HealthCheck";
        var healthCheckDataNode = document.createElement("div");
        healthCheckDataNode.innerText = checks[i].endpointName;
        healthCheckNode.appendChild(healthCheckDataNode);
        environmentNode.appendChild(healthCheckNode);

        xmlHttp.onreadystatechange = (function(getHealthCheck, displayElement) {
            return function() {
                if (getHealthCheck.readyState == 4 && getHealthCheck.status == 200) {
                    var healthResult = JSON.parse(getHealthCheck.responseText);
                    var healthResultFormat = JSON.stringify(healthResult, null, '    ');

                    for (var r = 0; r < healthResult.systemStates.length; r++) {
                        var subSystemElement = document.createElement("div");
                        subSystemElement.className = "SubSystem " + healthResult.systemStates[r].health;
                        subSystemElement.innerText = healthResult.systemStates[r].systemName;
                        displayElement.appendChild(subSystemElement);
                    }

                    //var pre = document.createElement("div");
                    //pre.className = "Result";
                    //pre.innerText = healthResultFormat;
                    //displayElement.appendChild(pre);
                    displayElement.className = healthResult.health;
                }
            }
        })(xmlHttp, healthCheckDataNode);

        xmlHttp.open("GET", checks[i].endPointUrl, true);
        xmlHttp.send();
    }
}*/
