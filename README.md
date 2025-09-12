# APHA-BST (BrainStem Training)

![Build status](https://github.com/DEFRA/apha-bst/actions/workflows/dev-ci.yaml/badge.svg)[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=DEFRA_apha-bst&metric=alert_status)](https://sonarcloud.io/dashboard?id=DEFRA_apha-bst)[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=DEFRA_apha-bst&metric=coverage)](https://sonarcloud.io/dashboard?id=DEFRA_apha-bst)

**APHA-BST** is the codebase for BST (BrainStem Training), built using ASP.NET Core MVC and hosted on AWS. 

---

## Table of Contents

- [Features](#features)  
- [Key technologies](#Keytechnologies)  
- [Deployment](#deployment)  
- [Versioning](#versioning)  
- [License](#license)

---

## Features

- Web application using ASP.NET Core MVC  
- Secure user authentication & authorization  
- Role-based access control  
- Training module workflow (assign, track, complete training)  
- Reporting / dashboards  
- Hosted on AWS for scalability and reliability  

---

## Key technologies

- ASP.NET Core MVC 
- RDS PostgreSQL 
- AWS Cloud infrastructure  
- ECR for CI
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

This project is licensed under the [MIT License](LICENSE).  
See the LICENSE file for full details.

