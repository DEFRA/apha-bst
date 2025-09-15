# APHA-BST (BrainStem Training)

![Build status](https://github.com/DEFRA/apha-bst/actions/workflows/dev-ci.yaml/badge.svg)[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=DEFRA_apha-bst&metric=alert_status)](https://sonarcloud.io/dashboard?id=DEFRA_apha-bst)[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=DEFRA_apha-bst&metric=coverage)](https://sonarcloud.io/dashboard?id=DEFRA_apha-bst)

**APHA-BST** is the codebase for BST (BrainStem Training), built using ASP.NET Core MVC and hosted on AWS. 

---

## Table of Contents

- [Features](#features)  
- [Technologies](#Technologies)  
- [Deployment](#Deployment)  
- [Versioning](#Versioning)  
- [License](#License)  
---

## Features

- Web application using ASP.NET Core MVC  
- Secure user authentication & authorization  
- Role-based access control  
- Training module workflow (assign, track, complete training)  
- Reporting / dashboards  
- Hosted on AWS for scalability and reliability  

---

## Technologies

- ASP.NET Core MVC 
- RDS SQL SERVER
- AWS Cloud infrastructure  
- ECR for application container image service
- ECS for hosting application 

---

## Deployment

- CI using GitHub Actions
- CD via Jenkins job

---

## Versioning  

This project uses [Semantic Versioning (SemVer)](https://semver.org/) for versioning.  
Image Versions are indicated by **Git tags** in the repository (e.g. `v1.2.3`).  

You can view all available versions directly in the GitHub UI:  

- **Releases page:**  
  [https://github.com/DEFRA/apha-bst/releases](https://github.com/DEFRA/apha-bst/releases)

- **Tags page:**  
  [https://github.com/DEFRA/apha-bst/tags](https://github.com/DEFRA/apha-bst/tags)

---

## License

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

>Contains public sector information licensed under the Open Government Licence v3.0.

### About the license

The Open Government Licence (OGL) v3.0 was developed by the The National Archives to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.


