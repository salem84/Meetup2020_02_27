import { check, group, sleep } from "k6";
import http from "k6/http";

import { Rate } from "k6/metrics";
const myFailRate = new Rate('Fail Rate');

// User scenario
export default function() {
    group("Get Random Meal", function() {
        let res = http.get("https://meetup2020.azurewebsites.net/api/v1/getrandommeal");

        check(res, {
            "is status 200": (r) => r.status === 200
        });

        myFailRate.add(res.error_code);

        sleep(5);
    });
}