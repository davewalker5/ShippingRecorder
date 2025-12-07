# ShippingRecorder

[![Build Status](https://github.com/davewalker5/ShippingRecorder/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/ShippingRecorder/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/ShippingRecorder)](https://github.com/davewalker5/ShippingRecorder/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/ShippingRecorder/badge.svg?branch=master)](https://coveralls.io/github/davewalker5/ShippingRecorder?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/ShippingRecorder.svg?include_prereleases)](https://github.com/davewalker5/ShippingRecorder/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/ShippingRecorder/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/ShippingRecorder/)
[![Language](https://img.shields.io/badge/database-SQLite-blue.svg)](https://github.com/davewalker5/ShippingRecorder/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/ShippingRecorder)](https://github.com/davewalker5/ShippingRecorder/)

## About

ShippingRecorder implements a SQL-based marine vessel sightings logbook. An ASP.NET WebAPI provides access to the business logic and data access layer while an ASP.NET MVC UI (currently in development) will provide the user interface.

### Reference Data

The application maintains details of:

- Countries
- Operators
- Ports
- Vessels
  - IMO
  - Physical characteristics
  - Registration history
    - Operator
    - Flag
    - MMSI
    - Vessel Name
    - Capacity details
- Voyages, including itinerary 

### Sightings

Each sighting consists of the following data:

- Location and date the sighting was made
- Vessel
- Voyage (optional)

### Search

Sightings may be searched by:

- Vessel
- Sighting sighting and date range
- Date range

## Getting Started

Please see the [Wiki](https://github.com/davewalker5/ShippingRecorder/wiki) for configuration details and the user guide.

## Authors

- **Dave Walker** - _Initial work_

## Credits

### JWT Authentication

Implementation of authentication using JWT in the REST API is based on the following tutorial:

- https://github.com/cornflourblue/aspnet-core-3-jwt-authentication-api
- https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api#users-controller-cs

### Reference Data

Sources of publicly-available and free-to-use reference data are as follows:

| Data Type     | Web Site | URL                                              |
| ------------- | -------- | ------------------------------------------------ |
| Country codes | DataHub  | https://datahub.io/core/country-list             |
| Ports         | UNCE     | https://unece.org/trade/cefact/UNLOCODE-Download |

## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/ShippingRecorder/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
