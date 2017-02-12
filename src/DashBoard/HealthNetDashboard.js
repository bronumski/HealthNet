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

        var popupId = endpointElement.id + "_RAW";
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

    var builErrorUnavailableResult = function(xmlHttpResponse, endpointElement) {
        var unavailableResult = {
            health: "Unavailable",
            status: xmlHttpResponse.status,
            message: xmlHttpResponse.responseText,
            systemStates: [],
        };
        updateEndpointHealthStatus(endpointElement, unavailableResult);
    }
    var setupHealthcheckCall = function(endpoint, endpointElement){
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.timeout = 10000;
        xmlHttp.onreadystatechange = function() {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    var healthResult = JSON.parse(this.responseText);
                    updateEndpointHealthStatus(endpointElement, healthResult);
                }
                else {
                    builErrorUnavailableResult(this, endpointElement);
                }
            }
        };
        xmlHttp.onerror = function() {
            builErrorUnavailableResult(this, endpointElement);
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
            var endpointElement = buildEndpointElement(env.environmentEndpoints[i]);
            endpointElement.id = "HealthNetDashBoard_" + env.id + "_" + i;
            environmentDashboard.appendChild(endpointElement)
        }
        return environmentDashboard;
    };

    var setupDashboard = function() {
        dashboardElement.id = "HealthNetDashBoard";
        dashboardElement.className = "HealthNetDashBoard";

        for (i = 0; i < environments.length; i++) {
            var environment = environments[i];
            environment.id = i;
            var environmentNode = buildEnvironmentElement(environment);
            environmentNode.id = dashboardElement.id + "_" + i;
            dashboardElement.appendChild(environmentNode);
        }
    };
    setupDashboard();
}

HealthNetDashboard.createEnvironment = function(name, endpoints) {
    return { environmentName: name, environmentEndpoints: endpoints };
};