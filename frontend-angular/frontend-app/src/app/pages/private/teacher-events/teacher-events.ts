import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { ModelListParticipants } from '../../../models/modelParticipant';
import { TeacherService } from '../../../services/teacher';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../../services/events';
import { AuthService } from '../../../services/authService';
import { ParticipantsService } from '../../../services/participants';
import { EventListResponse } from '../../../models/modelEvents';

@Component({
  selector: 'app-teacher-event',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink, ReactiveFormsModule],
  templateUrl: './teacher-events.html',
  styleUrls: ['./teacher-events.scss'],
})
export class TeacherEvents {
  course = signal<EventListResponse[]>([]); //Variable to receive course
  loadingCourse = signal<boolean>(false); //Loading view UI of course
  participants = signal<ModelListParticipants[]>([]); //List of participants
  loadingParticipants = signal<boolean>(false); //Loading view UI of participants
  errorLoadingParticipants: boolean = true; //Variable if the participant don't exist, will set false, remove every UI of participant

  editingId = signal<number | null>(null);
  editParticipants: FormGroup;

  private eventId: number = 0;

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private participantsService: ParticipantsService,
    private fb: FormBuilder
  ) {
    this.editParticipants = fb.group({
      grade: [
        '',
        [
          Validators.required,
          Validators.min(0),
          Validators.max(20),
          Validators.pattern(/^\d+(\.\d{1,2})?$/), //only numbers and decimals
        ],
      ],
      comments: ['', [Validators.maxLength(200)]],
    });
  }

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id')) || 0;
    const coursePassed = history.state?.['course'];

    if (!coursePassed || (coursePassed.Id ?? coursePassed.id) !== this.eventId) {
      return;
    }

    this.getEvent();
  }

  //Get the event from backend, from specific eventId
  private getEvent() {
    this.loadingCourse.set(true);
    this.eventService.getEventByEventId(this.eventId).subscribe({
      next: (res) => {
        if (!res) {
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
        this.getParticipants();
      },
    });
  }
  //Get the participants from backend, from specific eventId
  private getParticipants() {
    this.loadingParticipants.set(true);
    this.participantsService.getParticipantsIndividualEvent_teacher(this.eventId).subscribe({
      next: (res) => {
        if (Array.isArray(res) && !!res) {
          this.loadingParticipants.set(false);
          this.errorLoadingParticipants = false;
          this.participants.set(
            res.map((x: any) => ({
              Id: x.id ?? x.Id,
              EntityId: x.entityId ?? x.EntityId,
              Name: x.name ?? x.Name,
              Email: x.email ?? x.Email,
              Status: x.status ?? x.Status,
              Grade: x.grade ?? x.Grade,
              Comments: x.comments ?? x.Comments,
            }))
          );
          return;
        }
        this.participants.set([]);
        this.loadingParticipants.set(false);
        this.errorLoadingParticipants = false;
      },
    });
  }
  //Button to save the grade and comments of individual student in backend
  btnSaveEdit() {
    const payload = {
      Id: this.editingId()!,
      EventId: this.eventId,
      Grade: this.editParticipants.value.grade.toString(),
      Comments: this.editParticipants.value.comments,
    };

    if (!payload.Grade) {
      alert('You need to insert grade of student');
    }

    this.participantsService.insertParticipantGrade(payload).subscribe({
      next: (res) => {
        if (res) {
          alert('Participant grade update');
          this.btnCancelEdit();
          setTimeout(() => {
            this.getParticipants();
          }, 1000);
          this.editParticipants.markAsPristine();
          return;
        }
      },
    });
  }
  //Button to cancel editing of grade and comments
  btnCancelEdit() {
    this.editingId.set(null);
    this.editParticipants.reset();
  }
  //Button to edit grade and comments of individual student
  btnStartEdit(p: { Id: number; Grade?: string; Comments?: string }) {
    this.editingId.set(p.Id);
    this.editParticipants.setValue({
      grade: p.Grade ?? '',
      comments: p.Comments ?? '',
    });
  }
  //Button to inactive student from a event
  btnInactive(p: any) {
    console.log(p);
    const payload = {
      Id: p.Id,
      EventId: this.eventId,
      EntityId: p.EntityId,
    };

    this.participantsService.updateParticipantStatus(payload).subscribe({
      next: (res) => {
        if (res) {
          alert('Participant status inactive');
          setTimeout(() => {
            this.getParticipants();
          }, 1000);
          return;
        }
        return;
      },
    });
  }
  //! To be tested
  //Button to change status event - Close status
  btnCloseEvent() {
    const participants = this.participants();
    const course = this.course().filter((e) => e.Status === 'Ongoing');

    if (!course) {
      alert('Only ongoing events can be closed.');
      return;
    }

    if (participants.length === 0) {
      alert('You need participants to be able to close the event.');
      return;
    }

    const hasActive = participants.some((p) => p.Status === true);

    const hasNoGrade = participants.some(
      (p) =>
        p.Grade == null ||
        p.Grade === '' ||
        (typeof p.Grade === 'string' && p.Grade.trim().length === 0)
    );

    if (hasActive || hasNoGrade) {
      alert('All students need to be graded and set as inactive before closing the event.');
      return;
    }

    const payload = { Id: this.eventId };

    this.eventService.updateEventStatusClose(payload).subscribe({
      next: (res) => {
        if (res) {
          this.course.update((list) =>
            list.map((c) => (c.Id === this.eventId ? { ...c, Status: 'Close' } : c))
          );
        }
      },
    });
  }
  //Button to change status event - Ongoing status
  btnOngoingEvent() {
    const courseStatus = this.course().filter((e) => e.Status === 'Open');

    const payload = {
      Id: this.eventId,
    };

    if (this.participants().length == 0 && !!courseStatus) {
      alert('You need have students listed on the event.');
      return;
    }

    this.eventService.updateEventStatusOngoing(payload).subscribe({
      next: (res) => {
        if (res) {
          this.course.update((list) =>
            list.map((c) => (c.Id === this.eventId ? { ...c, Status: 'Ongoing' } : c))
          );
          return;
        }
      },
    });
  }
}
