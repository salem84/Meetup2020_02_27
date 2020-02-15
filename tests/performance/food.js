import { check, group, sleep } from "k6";
import http from "k6/http";
import { Rate } from "k6/metrics";

const myFailRate = new Rate('failed requests');
const apiRandomMeal = "https://www.themealdb.com/api/json/v1/1/random.php";
const apiBaseURL = "https://meetup2020.azurewebsites.net/api/v1";

// Test Configuration
const maxUsers = 1;
const sleepTimeSec = 1;
const randomMealLocal = true;
const isLogEnabled = true;

// Test configuration
export let options = {
    // Rampup for 10s from 1 to MAX, stay at MAX, and then down to 0
    stages: [
        { duration: "10s", target: maxUsers },
        { duration: "20s", target: maxUsers },
        { duration: "10s", target: 0 }
    ],
    thresholds: {
        "http_req_duration": ["p(95)<250"]
    }
};

// User scenario
export default function() {
    group("Add food", function() {        
        let meal = getRandomMeal();

        let payload = 
        {
            name: meal.strMeal,
            type: meal.strCategory,
            calories: Math.floor(Math.random() * 500),
            created: new Date().toISOString()
        };

        log("Random Meal: " + payload.name);

        let headers = { "Content-Type": "application/json" };

        let res = http.post(apiBaseURL + "/foods", JSON.stringify(payload), { headers: headers });

        check(res, {
            "is created (status 201)": (r) => r.status === 201
        });

        myFailRate.add(res.status !== 201);

        sleep(sleepTimeSec);
    });
}

function getRandomMeal() {
    if(randomMealLocal) {
        return {
            strMeal: "Cibo " + Math.floor(Math.random() * 500),
            strCategory: "Generico"
        }
    }
    else {
        let randomMeal = http.get(apiRandomMeal);
        log("Received random meal");
        return randomMeal.json().meals[0];
    }
}

function printConfiguration() {
    console.log("------- Configuration -------");
    console.log("Random meal Local: " + randomMealLocal);
}

function log(s) {
    if(isLogEnabled) {
        console.log(s);
    }
}