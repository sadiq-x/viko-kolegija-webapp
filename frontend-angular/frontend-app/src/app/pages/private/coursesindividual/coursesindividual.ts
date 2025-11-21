import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ParticipantsService } from '../../../services/participants';
import { ModelListParticipantInfo } from '../../../models/modelParticipant';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Location } from '@angular/common';
import { AuthService } from '../../../services/authService';
import { EventListResponse } from '../../../models/modelEvents';
import { EventService } from '../../../services/events';

@Component({
  selector: 'app-coursesindividual',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './coursesindividual.html',
  styleUrl: './coursesindividual.scss',
})
export class CoursesIndividual {
  courseType = { Type: '' }; //Variable type of topic course
  course = signal<EventListResponse[]>([]); //Variable to receive course
  loadingCourse = signal<boolean>(false); //Loading view UI of course
  participant = signal<ModelListParticipantInfo[]>([]); //Variable to receive participant
  participantEventStatus: boolean = true; //Variable if the participant don't are registered, will set false, remove every UI of participant
  formEvent!: FormGroup; //form of Description input

  loginType: string | null = null; //Variable type of login
  
  private eventId: number = 0;
  private coursePassed: any;

  constructor(
    private route: ActivatedRoute,
    private participantsService: ParticipantsService,
    private eventService: EventService,
    private authService: AuthService,
    private fb: FormBuilder,
    private location: Location,
    private router: Router
  ) {
    this.formEvent = this.fb.group({
      description: [''],
    });
  }

  ngOnInit() {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.coursePassed = history.state?.['event'];
    this.courseType.Type = this.coursePassed.TopicName;

    if (!this.coursePassed && !this.eventId && !this.courseType.Type) {
      return;
    }

    this.getEvent();
    this.loginType = this.authService.getRole();
    
  }

  //Get the event from backend, from specific eventId
  private getEvent() {
    this.loadingCourse.set(true);

    this.eventService.getEventByEventId(this.eventId).subscribe({
      next: (res) => {
        if (!res) {
          this.loadingCourse.set(false);
          this.course.set([]);
          return;
        }

        const x = res as any;
        this.course.set([
          {
            Id: x.Id ?? x.id,
            Name: x.Name ?? x.name,
            Description: x.Description ?? x.description,
            TopicName: x.TopicName ?? x.topicName,
            CreateBy: x.CreateBy ?? x.createBy,
            DateCreate: x.DateCreate ?? x.dateCreate,
            DateClose: x.DateClose ?? x.dateClose,
            Status: x.Status ?? x.status,
          },
        ]);
        this.loadingCourse.set(false);
        this.getParticipant();
      },
    });
  }
  //Get the information of a individual participant
  private getParticipant() {
    this.participantsService.getParticipantsIndividualEvent_user(this.eventId).subscribe({
      next: (res) => {
        if (!res) {
          this.participant.set([]);
          this.participantEventStatus = false;
          return;
        }

        if (Array.isArray(res)) {
          this.participant.set(
            res.map((e: any) => ({
              Grade: e.Grade || e.grade,
              Comments: e.Comments || e.comments,
              ParticipantDescription: e.ParticipantDescription || e.participantDescription,
              Status: e.Status || e.status,
            }))
          );
          return;
        }
      },
    });
  }
  //Button to register on event
  btnRegisterEvent() {
    const payload = {
      EventId: this.eventId,
    };

    if (!payload.EventId && this.loginType != 'User') {
      return;
    }

    this.participantsService.insertParticipantsInEvent(payload).subscribe({
      next: (res) => {
        if (!res) {
          this.participantEventStatus = false;
          return;
        }

        this.participantEventStatus = true;
        this.getParticipant();
      },
    });
  }
  //Button to cancel registration on event
  btnCancelEvent() {
    const payload = {
      EventId: this.eventId,
    };
    const participantGrade = this.participant().filter(c => c.Grade === '' )
    const participantStatus = this.participant().filter(c => c.Status === true )

    if (!payload.EventId && this.loginType != 'User' && !participantGrade && !participantStatus) {
      return;
    }

    this.participantsService.cancelParticipantStatus(payload).subscribe({
      next: (res) => {
        if (!res) {
          this.participantEventStatus = false;
          return;
        }

        this.participantEventStatus = true;
        this.getParticipant();
      },
    });

  }
  //Button to insert description of student on event
  btnInsertParticipantDescription() {
    const payload = {
      EventId: this.eventId,
      ParticipantDescription: this.formEvent.get('description')?.value,
    };

    if (!payload.ParticipantDescription || payload.ParticipantDescription.trim().length === 0) {
      alert('You need to write your appointments');
      return;
    }

    this.participantsService.insertParticipantParticipantDescription(payload).subscribe({
      next: (res) => {
        console.log(res);
        if (!res) {
          alert("The actual description isn't saved");
          return;
        }

        this.participantEventStatus = true;
        this.participant.update((current) =>
          current.map((item) => ({
            ...item,
            ParticipantDescription: payload.ParticipantDescription,
          }))
        );

        this.formEvent.reset();
      },
    });
  }
  goBack(event: Event) {
    event.preventDefault();

    if (window.history.length > 2) {
      this.location.back();
      return;
    }

    if (this.courseType?.Type) {
      this.router.navigate(['/courses/type', this.courseType.Type], {
        state: { course: this.courseType },
      });
    } else {
      this.router.navigate(['/course']);
    }
  }
  get cancelButtonDisabled() {
  const p = this.participant();
  const c = this.course();

  const missingDescription = p.some(c => !c.ParticipantDescription || c.ParticipantDescription.trim() === '');
  const missingGrade = p.some(c => !c.Grade || c.Grade.trim() === '');
  const hasActiveStatus = p.some(c => c.Status === true);
  const hasEventOpenStatus = c.some(c => c.Status === "Open");

  return missingDescription && missingGrade && hasActiveStatus && hasEventOpenStatus;
}
}
