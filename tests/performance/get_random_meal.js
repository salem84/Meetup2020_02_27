import { check, group, sleep } from "k6";
import http from "k6/http";

// User scenario
export default function() {
    group("Get Random Meal", function() {
        // Make a request for the front page HTML (this will not fetch static resources referenced by HTML file)
        let res = http.get("https://meetup2020.azurewebsites.net/api/v1/getrandommeal");

        // Make sure the status code is 200 OK
        check(res, {
            "is status 200": (r) => r.status === 200
        });

        // Simulate user reading the page
        sleep(5);
    });
}