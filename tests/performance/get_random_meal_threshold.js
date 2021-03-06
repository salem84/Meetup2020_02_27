import { check, group, sleep } from "k6";
import http from "k6/http";

import { Rate } from "k6/metrics";
const myFailRate = new Rate('custom_fail_rate');

export let options = {
    vus: 10,
    duration: "30s",
    thresholds: {
        "custom_fail_rate": ["rate < 0.2"]
    }
};

// User scenario
export default function() {
    group("Get Random Meal", function() {
        let res = http.get("https://meetup2020.azurewebsites.net/api/v1/foods/getrandommeal");

        check(res, {
            "is status 200": (r) => r.status === 200
        });
        myFailRate.add(res.status == 503);

        sleep(5);
    });
}