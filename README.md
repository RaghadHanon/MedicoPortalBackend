# MedicoPortalAPI 

This API offers a suite of endpoints for managing MedicoPortal activities. 

This project is designed to streamline healthcare management by providing a comprehensive system for patient management and medical records. MedicoPortal aims to enhance the healthcare experience for both patients and providers through a robust set of features tailored to meet the diverse needs of the medical community.

MedicoPortal provides an intuitive and secure platform where patients can manage their health information, and healthcare providers can efficiently handle patient data and medical records. Our system ensures that critical medical information is easily accessible and up-to-date, fostering better communication and decision-making between patients and their healthcare providers.



## MedicoPortal website
- [MedicoPortal](https://softwareproject-w3yf.onrender.com/)


  
## Documentation
### Swagger Documentation

- **MedicoPortal System API**
  - [API Documentation](https://medicalportal.runasp.net/swagger/index.html)
 
### SRS Documentation

- **MedicoPortal SRS**
  - [SRS Documentation](https://drive.google.com/file/d/17aBaKFu-sR-lxUW4pH9LwZQVOasAd4cR/view?usp=sharing)



## Illustration vedio 
- [MedicoPortal guide vedio](https://drive.google.com/file/d/1QlMdOefKHsPtD-sCjVeVMpC4JDylG5B5/view?usp=drive_link)




## ⭐ Key Features

### Doctors Management:
- **Add Doctor**: 🩺 Allows a user to register as a doctor.
- **Get Doctor**: 🔍 Retrieve details of a specific doctor by their username.
- **Update Profile Info**: ✏️ Modify a doctor's personal information.
- **List Doctors**: 📋 Retrieve a list of all doctors in the system.
- **List Doctors Based on Medical Specializations**: 🏥 Retrieve a list of doctors with specific medical specializations.
- **Update Clinic Info**: 🏢 Modify the details of a doctor's clinic if it exists.
- **List Requests**: 📥 Retrieve a list of all requests received by the doctor, categorized based on their state (answered, not answered).
- **Get Request Details**: 📝 Retrieve details of a specific request by its ID, including the details of the response to this request if it exists.

  
### Patient Management:
- **Add Patient**: 🧑‍⚕️ Allows the user to register as a patient.
- **Get Patient**: 🗂️ Retrieve the personal information of a specific patient.
- **Update Profile Info**: ✏️ Modify a patient's personal information.
- **List Requests**: 📜 Retrieve a list of all requests sent to doctors, categorized based on their state (answered, not answered).
- **Create Request**: 📩 Send a request to a specific doctor by their ID.
- **Get Request Details**: 📝 Retrieve details of a specific request by its ID, including the details of the response to this request if it exists.
- **Add Blood Pressure Measurement**: 💉 Add a new blood pressure measurement.
- **List Blood Pressure Measurements**: 🍬 Retrieve a detailed list of blood pressure measurements by patient user name.
- **Add Blood Sugar Measurement**: 💉 Add a new blood sugar measurement.
- **List Blood Sugar Measurements**: 🍬 Retrieve a detailed list of blood sugar measurements by patient user name.
- **List Allergies**: 🌿 Retrieve a detailed list of allergies doctors diagnose by patient user name.
- **List Chronic Diseases**: 🩸 Retrieve a detailed list of chronic diseases doctors diagnose by patient user name.
- **List Medicines**: 💊 Retrieve a detailed list of medicines added by doctors by patient user name.

### Medical Specification Management:
- **Create Medical Specification:** 🆕Add a new Medical Specification to the system.
- **List Medical Specifications:** 📜 Retrieve a list of Medical Specifications.
- **Delete Medical Specification:** 🗑️Remove a Medical Specification from the system.

### Response to Request Management:
- **Create Response:** 🆕Add a new response containing a general Report, Allergy, Medicine, and Chronic Disease to the profile of the patient who sent the request.


## Endpoints

### Authentication Endpoints

| HTTP Method | Endpoint              | Description           |
|-------------|-----------------------|-----------------------|
| POST        | `/api/Authentication/register-doctor`       | Register a doctor  |   
| POST        | `/api/Authentication/register-patient` | Register a patient     |
| POST        | `/api/Authentication/register-admin` | Register an admin     |
| POST        | `/api/Authentication/login`          | Login a user          |

### Doctor Endpoints

| HTTP Method | Endpoint             | Description                              |
|-------------|----------------------|------------------------------------------|
| GET         | `/api/doctors`                  | Retrieve list of doctors      |
| GET         | `/api/medicalSpecification/{medicalSpecificationId}/doctors`    | Retrieve a list of doctors with specific medical specializations.               |
| GET         | `/api/doctor/{userName}`        | Get a doctor by its userName  |
| POST        | `/api/doctor/clinic`            | Create a clinic               |    
| POST         | `/api/doctor/profileData`      | Add profile data              |
| GET        | `/api/doctor/requests`                       |  Retrieve a list of all requests received by the doctor, categorized based on their state (answered, not answered)|
| GET         | `/api/doctor/requests/{requestid}`                       | Retrieve details of a specific request by its ID, including the details of the response to this request if it exists|

### Patient Endpoints

| HTTP Method | Endpoint             | Description                              |
|-------------|----------------------|----------------------------------------- |
| GET         | `/api/patient/{userName}`        | Get a patient detailes by its userName  |  
| POST         | `/api/patient/profileData`      | Add profile data              |
| GET        | `/api/patient/requests`                       | Retrieve a list of all requests sent to doctors, categorized based on their state (answered, not answered)|
| GET         | `/api/patient/requests/{requestid}`                       | Retrieve details of a specific request by its ID, including the details of the response to this request if it exists|
| POST         | `/api/patient/request/{doctorid}`                       | Send a request to a specific doctor by their ID|
| POST         | `/api/patient/bloodPressure`                       | Add a new blood pressure measurement|
| POST         | `/api/patient/bloodSugar`                       |  Add a new blood sugar measurement|
| GET        | `/api/patient/bloodPressures/{userName}`      |Retrieve a detailed list of blood pressure measurements by patient user name|
| GET        | `/api/patient/bloodSugars/{userName}`                       | Retrieve a detailed list of blood sugar measurements by patient user name|
| GET        | `/api/patient/allergies/{userName}`                       |Retrieve a detailed list of allergies doctors diagnose by patient user name|
| GET        | `/api/patient/chronicDiseases/{userName}`                       | Retrieve a detailed list of chronic diseases doctors diagnose by patient user name|
| GET        | `/api/patient/medicines/{userName}`                       | Retrieve a detailed list of medicines added by doctors by patient user name|

   
### Medical Specification Endpoints:

| HTTP Method | Endpoint             | Description                              |
|-------------|----------------------|----------------------------------------- |
| POST        | `/api/MedicalSpecification`      |Add a new Medical Specification to the system.
| GET        | `/api/MedicalSpecification`      | Retrieve a list of Medical Specifications.
| DELETE        | `/api/MedicalSpecification`      | Remove a Medical Specification from the system.


### Response Endpoints:
| HTTP Method | Endpoint             | Description                              |
|-------------|----------------------|----------------------------------------- |
|Post | `/api/response` |Add a new response containing a general Report, Allergy, Medicine, and Chronic Disease to the profile of the patient who sent the request|



## Tools and Concepts

This section provides an overview of the key tools, technologies, and concepts used in the development.

### Programming Languages and Frameworks
- **C#**: Primary programming language used.
- **.NET Core**: Framework for building high-performance, cross-platform web APIs.

### Database
- **Entity Framework Core**: Object-relational mapping (ORM) framework for .NET.
- **SQL Server**: Database management system used for storing all application data.


### API Documentation and Design
- **Swagger/OpenAPI**: Used for API specification and documentation.
- **Swagger UI**: Provides a web-based UI for interacting with the API's endpoints.

### Authentication and Authorization
- **JWT (JSON Web Tokens)**: Method for securely transmitting information between parties as a JSON object.

### Security
- **HTTPS**: Ensuring secure communication over the network.
- **Data Encryption**: Encrypting sensitive data in the database.

Thank you for your interest.
