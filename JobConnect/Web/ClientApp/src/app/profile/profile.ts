import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Select2Data } from 'ng-select2-component';
import { ModalService } from '../common/app-modal/app-modal-service';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { CommonService } from '../common/services/common-service';
import { MessageService } from '../common/services/message-service';
import { UserService } from '../common/services/user-service';


@Component({
  selector: 'app-profile',
  templateUrl: './profile.html'
})
export class Profile implements OnInit {
  // user info
  displayName = '';
  userProfileImage = '';
  userProfileImageId = 0;
  genderId = 0;
  maritalStateId = 0;
  militaryStateId = 0;
  cityId = 0;
  stateId = 0
  salaryRequestedId = 0;
  genders: Select2Data = [];
  maritalStatus: Select2Data = [];
  militaryStatus: Select2Data = [];
  cities: Select2Data = [];
  salaryRequesteds: Select2Data = [];
  infoProfile = new FormGroup({
    firstName: new FormControl('', Validators.required),
    lastName: new FormControl('', Validators.required),
    phoneNumber: new FormControl(''),
    email: new FormControl(''),
    birthDate: new FormControl(''),
    genderId: new FormControl(0),
    maritalStateId: new FormControl(0),
    militaryStateId: new FormControl(0),
    cityId: new FormControl(0),
    stateId: new FormControl(0),
    salaryRequestedId: new FormControl(0),
    userProfileImageId: new FormControl(0),
    description: new FormControl('')
  });

  //user education
  degreeId = 0;
  degrees: Select2Data = [];
  userEducations: any = [];
  saveEducationForm = new FormGroup({
    id: new FormControl(0),
    field: new FormControl('', Validators.required),
    universityName: new FormControl('', Validators.required),
    degreeId: new FormControl(''),
    startDate: new FormControl(''),
    endDate: new FormControl(''),
    score: new FormControl(0),
    description: new FormControl(''),
  });

  //user job
  positionId = 0;
  jobGroupId = 0;
  jobCityId = 0;
  jobPositions: Select2Data = [];
  jobGroups: Select2Data = [];
  userJobs = [];
  saveJobForm = new FormGroup({
    id: new FormControl(0),
    jobTitle: new FormControl(''),
    companyName: new FormControl(''),
    description: new FormControl(''),
    startDate: new FormControl(''),
    endDate: new FormControl(''),
    isCurrentJob: new FormControl(false)
  });

  // user languages
  userLanguages = [];
  languages: Select2Data = [];
  languageLevels: Select2Data = [];
  languageLevelId = 0;
  languageId = 0;
  saveLanguageForm = new FormGroup({
    id: new FormControl(0),
    languageId: new FormControl(0),
    levelId: new FormControl(0)
  });


  //user skill
  userSkills = [];
  skills: Select2Data = [];
  skillLevels: Select2Data = [];
  skillId = 0;
  skillLevelId = 0;


  //user priority
  priorityForm = new FormGroup({
    id: new FormControl(0),
    languageId: new FormControl(0),
    levelId: new FormControl(0)
  });
  contractTypes: Select2Data = [];
  jobBenefits: Select2Data = [];

  selectedSalaryRequestedId: 0;
  selectedCities = '';
  selectedGroups = '';
  contractTypeIds = '';
  jobBenefitsIds = '';


  constructor(private userService: UserService,
    private commonService: CommonService,
    private http: HttpClient,
    private messageService: MessageService,
    protected modalService: ModalService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
  }
  ngOnInit() {
    const authUser = this.userService.getAuthUser();
    if (authUser) {
      this.displayName = authUser.displayName;
    }

    this.http.get(`${this.appConfig.apiEndpoint}/userProfile/get`).subscribe((response: any) => {

      //user info
      this.infoProfile.patchValue(
        {
          firstName: response.firstName,
          lastName: response.lastName,
          birthDate: response.birthDate,
          email: response.email,
          phoneNumber: response.phoneNumber,
          description: response.description
        });

      if (response.genderId) {
        this.genderId = response.genderId;
      }
      if (response.maritalStateId) {
        this.maritalStateId = response.maritalStateId;
      }
      if (response.militaryStateId) {
        this.militaryStateId = response.militaryStateId;
      }
      if (response.salaryRequestedId) {
        this.salaryRequestedId = response.salaryRequestedId;
      }
      if (response.cityId) {
        this.cityId = response.cityId;
      }
      if (response.userProfileImage) {
        //this.userProfileImage = `data:image/gif;base64,${response.userProfileImage}`;
        this.userProfileImage = `${response.userProfileImage}`;
        this.userProfileImageId = response.userProfileImageId;
      }
      else {
        this.userProfileImage = '../assets/images/jobs-company/pic1.jpg';
      }

      this.cities = response.cites;
      this.genders = response.genders;
      this.maritalStatus = response.maritalStatus;
      this.militaryStatus = response.militaryStatus;
      this.salaryRequesteds = response.salaryRequesteds;

      //user education
      this.degrees = response.degrees;
      this.userEducations = response.userEducations;

      //user job
      this.userJobs = response.userJobs;
      this.jobPositions = response.jobPositions;
      this.jobGroups = response.jobGroups;

      //user language
      this.userLanguages = response.userLanguages;
      this.languages = response.languages;
      this.languageLevels = response.languageLevels;

      //user skill
      this.skills = response.skills;
      this.skillLevels = response.skillLevels;
      this.userSkills = response.userSkills;

      // user  priorities
      this.contractTypes = response.contractTypes;
      this.jobBenefits = response.jobBenefits;
      if (response.userPriorities) {
        this.selectedSalaryRequestedId = response.userPriorities.salaryRequestedId;
        this.selectedCities = response.userPriorities.cities;
        this.selectedGroups = response.userPriorities.groups;
        this.contractTypeIds = response.userPriorities.contractTypes;
        this.jobBenefitsIds = response.userPriorities.benefits;
      }
    });
  }

  //user profile
  uploadProfile(event: any) {
    const file = event.target.files[0];
    const formData = new FormData();
    formData.append("file", file, file.name);
    formData.append("userProfileImageId", `${this.userProfileImageId}`);

    const reader = new FileReader();
    reader.readAsDataURL(event.target.files[0]);
    reader.onload = (e: any) => {
      this.userProfileImage = e.target.result
    }

    this.http.post(`${this.appConfig.apiEndpoint}/userProfile/uploadProfile`, formData).subscribe((response: any) => {
      if (response.isValid) {
        this.userProfileImageId = response.id;
      }
      else {
        this.userProfileImage = '';
      }
    });
  }
  saveProfile() {
    if (!this.infoProfile.valid) {
      return;
    }
    const item = this.infoProfile.value as any;
    item.genderId = this.genderId;
    item.maritalStateId = this.maritalStateId;
    item.militaryStateId = this.militaryStateId;
    item.cityId = this.cityId;
    item.salaryRequestedId = this.salaryRequestedId;
    item.stateId = this.stateId;

    this.http.post(`${this.appConfig.apiEndpoint}/userProfile/save`, item).subscribe((response: any) => {
      if (response.isValid) {
        this.messageService.success(response.message);
      }
    });
  }

  //user education
  educationModal(item) {
    if (item) {
      this.degreeId = item.degreeId;
      this.saveEducationForm.patchValue(item);
    }
    else {
      this.degreeId = null;
      this.saveEducationForm.reset();
    }
    this.modalService.open('education-save');
  }
  getUserEducation() {
    this.http.get(`${this.appConfig.apiEndpoint}/userEducation/get`).subscribe((response: any) => {
      this.userEducations = response;
    });
  }
  saveEducation() {
    if (!this.saveEducationForm.valid) {
      return;
    }
    const item = this.saveEducationForm.value as any;
    item.degreeId = this.degreeId?? item.degreeId;
    this.http.post(`${this.appConfig.apiEndpoint}/userEducation/save`, item).subscribe((response: any) => {
      if (response.isValid) {
        this.messageService.success(response.message);
        this.modalService.close();
        this.getUserEducation();
        return;
      }
      return this.messageService.error(response.message);
    });
  }
  deleteEducation(id) {
    const item = {
      id: id
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userEducation/delete`, item).subscribe((response: any) => {
      if (response.isValid) {
        const findIndex = this.userEducations.findIndex(w => w.id == id);
        if (findIndex != null) {
          this.userEducations.splice(findIndex, 1);
        }
        return;
      }
      return this.messageService.error(response.message);
    });
  }

  //user job
  jobModal(item) {
    if (item) {
      this.positionId = item.positionId;
      this.jobGroupId = item.jobGroupId;
      this.jobCityId = item.jobGroupId;
      this.saveJobForm.patchValue(item);
    }
    else {
      this.positionId = null;
      this.jobGroupId = null;
      this.jobCityId = null;
      this.saveJobForm.reset();
    }
    this.modalService.open('job-save');
  }
  getUserJob() {
    this.http.get(`${this.appConfig.apiEndpoint}/userJob/get`).subscribe((response: any) => {
      this.userJobs = response;
    });
  }
  saveJob() {
    if (!this.saveJobForm.valid) {
      return;
    }
    const item = this.saveJobForm.value as any;
    item.positionId = this.positionId?? item.positionId;
    item.jobGroupId = this.jobGroupId?? item.jobGroupId;
    item.cityId = item.cityId ?? this.jobCityId;

    this.http.post(`${this.appConfig.apiEndpoint}/userJob/save`, item).subscribe((response: any) => {
      if (response.isValid) {
        this.messageService.success(response.message);
        this.modalService.close();
        this.getUserJob();
        return;
      }
      return this.messageService.error(response.message);
    });
  }
  deleteJob(id) {
    const item = {
      id: id
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userJob/delete`, item).subscribe((response: any) => {
      if (response.isValid) {
        const findIndex = this.userJobs.findIndex(w => w.id == id);
        if (findIndex != null) {
          this.userJobs.splice(findIndex, 1);
        }
        return;
      }
      return this.messageService.error(response.message);
    });
  }

  //user language
  languageModal(item) {
    if (item) {
      this.languageId = item.languageId;
      this.languageLevelId = item.levelId;
      this.saveLanguageForm.patchValue(item);
    }
    else {
      this.languageId = null;
      this.languageLevelId = null;
      this.saveLanguageForm.reset();
    }
    this.modalService.open('language-save');
  }
  getUserLanguage() {
    this.http.get(`${this.appConfig.apiEndpoint}/userLanguage/get`).subscribe((response: any) => {
      this.userLanguages = response;
    });
  }
  saveLanguage() {
    if (!this.saveLanguageForm.valid) {
      return;
    }
    const item = this.saveLanguageForm.value as any;
    item.languageId = this.languageId ?? item.languageId;
    item.levelId = this.languageLevelId ?? item.levelId;

    this.http.post(`${this.appConfig.apiEndpoint}/userLanguage/save`, item).subscribe((response: any) => {
      if (response.isValid) {
        this.messageService.success(response.message);
        this.modalService.close();
        this.getUserLanguage();
        return;
      }
      return this.messageService.error(response.message);
    });
  }
  deleteLanguage(id) {
    const item = {
      id: id
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userLanguage/delete`, item).subscribe((response: any) => {
      if (response.isValid) {
        const findIndex = this.userLanguages.findIndex(w => w.id == id);
        if (findIndex != null) {
          this.userLanguages.splice(findIndex, 1);
        }
        return;
      }
      return this.messageService.error(response.message);
    });
  }

  //user skill
  getUserSkill() {
    this.http.get(`${this.appConfig.apiEndpoint}/userSkill/get`).subscribe((response: any) => {
      this.userSkills = response;
    });
  }
  saveSkill() {
    const item = {
      softwareSkillId: this.skillId,
      levelId: this.skillLevelId
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userSkill/save`, item).subscribe((response: any) => {
      if (response.isValid) {
        this.messageService.success(response.message);
        this.getUserSkill();
        return;
      }
      return this.messageService.error(response.message);
    });
  }
  deleteSkill(id) {
    const item = {
      id: id
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userSkill/delete`, item).subscribe((response: any) => {
      if (response.isValid) {
        const findIndex = this.userSkills.findIndex(w => w.id == id);
        if (findIndex != null) {
          this.userSkills.splice(findIndex, 1);
        }
        return;
      }
      return this.messageService.error(response.message);
    });
  }

  //user priority
  savePriority() {
    const item = {
      cities: this.selectedCities,
      groups: this.selectedGroups,
      contractTypes: this.contractTypeIds,
      benefits: this.jobBenefitsIds,
      salaryRequestedId: this.selectedSalaryRequestedId
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userPriority/save`, item).subscribe((response: any) => {
      if (response.isValid) {
        return this.messageService.success(response.message);
      }
      return this.messageService.error(response.message);
    });
  }

  setOption(type: number, data: any) {

    if (data.value) {
      data.component.isOpen = false;
      switch (type) {
        case 1:
          this.genderId = data.value;
          break;
        case 2:
          this.maritalStateId = data.value;
          break;
        case 3:
          this.militaryStateId = data.value;
          break;
        case 4:
          this.cityId = data.value;
          this.stateId = data.options[0].data;
          break;
        case 5:
          this.salaryRequestedId = data.value;
          break;
        case 6:
          this.degreeId = data.value;
          break;
        case 7:
          this.positionId = data.value;
          break;
        case 8:
          this.jobCityId = data.value;
          break;
        case 9:
          this.jobGroupId = data.value;
          break;
        case 10:
          this.languageId = data.value;
          break;
        case 11:
          this.languageLevelId = data.value;
          break;
        case 12:
          this.skillId = data.value;
          break;
        case 13:
          this.skillLevelId = data.value;
          break;
        case 14:
          this.selectedCities = data.value;
          break;
        case 15:
          this.selectedGroups = data.value;
          break;
        case 16:
          this.contractTypeIds = data.value;
          break;
        case 17:
          this.selectedSalaryRequestedId = data.value;
          break;
        case 18:
          this.jobBenefitsIds = data.value;
          break;
      }
    }
  }
}
