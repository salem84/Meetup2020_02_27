import { check, group, sleep } from "k6";
import http from "k6/http";

// User scenario
export default function() {
    group("Front page", function() {
        // Make a request for the front page HTML (this will not fetch static resources referenced by HTML file)
        let res = http.get("http://test.loadimpact.com/");

        // Make sure the status code is 200 OK
        check(res, {
            "is status 200": (r) => r.status === 200
        });

        // Simulate user reading the page
        sleep(5);
    });
}