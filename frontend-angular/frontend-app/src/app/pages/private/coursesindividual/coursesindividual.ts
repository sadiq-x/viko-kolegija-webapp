import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ParticipantsService } from '../../../services/participants';
import { ModelListParticipantInfo } from '../../../models/modelParticipant';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { isEmpty } from 'rxjs';

@Component({
  selector: 'app-coursesindividual',
  imports: [CommonModule, RouterLink, FormsModule, ReactiveFormsModule],
  templateUrl: './coursesindividual.html',
  styleUrl: './coursesindividual.scss',
})
export class CoursesIndividual {
  //Type of topic course
  courseType = {
    Type: '',
  };
  //State of loading and not found
  loading = signal(true);
  notFound = signal(false);

  //Content of event
  event = signal<any | null>(null);
  //Id event
  idCourse: number = 0;

  //Boolean status of button register
  participantEventStatus: boolean = true;

  //Information of participant
  participantInfo = signal<ModelListParticipantInfo[]>([]);

  formEvent!: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private participantsService: ParticipantsService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.loadCourse();
    this.loadParticipantInformation();
    this.formEvent = this.fb.group({
      description: [''],
    });
  }
  //Function to load event
  loadCourse() {
    this.loading.set(true);
    const passed = history.state?.['event'];

    if (!passed) {
      this.loading.set(false);
      this.notFound.set(true);
      return;
    }
    this.courseType.Type = passed.TopicName;

    this.idCourse = Number(this.route.snapshot.paramMap.get('id'));

    this.loading.set(false);
    this.event.set(passed);
  }
  //Get the information of a individual participant
  loadParticipantInformation() {
    if (this.idCourse <= 0 || !this.idCourse) {
      return;
    }

    this.participantsService.getParticipantsIndividualEvent(this.idCourse).subscribe({
      next: (res) => {
        console.log(res)
        if (res === false) {
          this.participantEventStatus = res;
          return;
        }
        if (Array.isArray(res)) {
          this.participantInfo.set(
            res.map((e: any) => ({
              Grade: e.Grade || e.grade,
              Comments: e.Comments || e.comments,
              ParticipantDescription: e.ParticipantDescription || e.participantDescription,
            }))
          );
        }
        return;
      },
    });
  }
  //Button to exit of event - not used
  btnCloseEvent() {}
  //Button to register on event
  btnRegisterEvent() {
    const obj = {
      eventId: this.idCourse,
    };

    this.participantsService.insertParticipantsInEvent(obj).subscribe({
      next: (res) => {
        if (res) {
          this.participantEventStatus = res;
          this.loadParticipantInformation();
        }
      },
    });
  }
  //Button to insert description of student on event
  btnInsertParticipantDescription() {
    const obj = {
      eventId: this.idCourse,
      participantDescription: this.formEvent.get('description')?.value,
    };
    console.log(obj)
    if (!obj.participantDescription || obj.participantDescription.trim().length === 0) {
      alert('You need to write your appointments');
      return;
    }

    this.participantsService.insertParticipantParticipantDescription(obj).subscribe({
      next: (res) => {
        console.log(res);
        if (res) {
          this.participantEventStatus = res
          this.participantInfo.update(current =>
          current.map(item => ({
            ...item,
            ParticipantDescription: obj.participantDescription,
          }))
        );

          this.formEvent.reset();
          return;
        } else {
          alert("The actual description isn't saved");
          return;
        }
      },
    });
  }
}
