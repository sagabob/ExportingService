[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="#">
    <img src="https://i.ibb.co/gb2tf3s/Tdp-logo-main.png" alt="Logo" width="285" height="170">
  </a>

  <h2 align="center">Exporting Service</h2>

  <p align="center">
    A simple event driven architecture system exporting Html pages to Pdf/Word   
  </p>
</p>

## Table of Contents

* [About the Project](#about-the-project)
* [Architecture Diagram](#Diagram)
* [Built with](#built-with)
* [CI/CD](#CI/CD)
* [Progress](#Progress)
* [Roadmap](#roadmap)
* [Contact](#contact)

## About the project
A simple event driven architecture system exporting Html pages to Pdf/Word. Users just send a request containing a list of URLs, target format (Pdf/Word) and email address. It components will received the request, converting Html pages to the target format, and then email to the user with exported file attached.   

## Diagram
<img src="https://i.imgur.com/PbBPHAe.png" alt="Architecture Diagram" width="800" height="560">

## Built With
* DotNet Core 7.
* Azure Storage Services (Tables & Blob).
* Azure Service Bus (Queue).
* Built with best security practices, Azure RBAC is used with service principal.
* TDD is used to drive the development.
* XUnit and Moq are used for testing.

## CI/CD
[![Build Status](https://dev.azure.com/bobpham-tdp-saga/TdpAGISApp/_apis/build/status%2FExporting_Service?branchName=main)](https://dev.azure.com/bobpham-tdp-saga/TdpAGISApp/_build/latest?definitionId=48&branchName=main)

## Progress
* Write all tests, aim to get near 100% test coverage.
  
## Roadmap
The application will be implemented with the following features:
* Use docker.
* Use Kubernetes.
* Deploy the whole system to Azure.

Other potential implementations:
* Use MassTransit to abstract persistence layer (Azure Table, Storage & Service Bus).
* Deploy the whole system to AWS.
* Or use serverless functions & app logic to do the exporting and emailing.

[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/bob-pham-93937973/
[tdp-logo]: tdp-logo.png
