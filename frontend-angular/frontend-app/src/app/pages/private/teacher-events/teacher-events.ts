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

@Component({
  selector: 'app-teacher-event',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink, ReactiveFormsModule],
  templateUrl: './teacher-events.html',
  styleUrls: ['./teacher-events.scss'],
})
export class TeacherEvents {
  course: any = null;
  loading = true;

  errorLoadPage: boolean = false;

  participants = signal<ModelListParticipants[]>([]);
  loadingParticipants = signal<boolean>(false);

  editingId = signal<number | null>(null);
  editParticipants: FormGroup;

  eventId: number = 0;

  constructor(
    private route: ActivatedRoute,
    private teacherService: TeacherService,
    private eventService: EventService,
    private participantsService: ParticipantsService,
    private fb: FormBuilder,

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
    this.getEventFromTeacherPage();
  }

  //Get the event selected from Page teacher
  getEventFromTeacherPage() {
    const passed = history.state?.['course'];
    
    //! SEE THIS
    //Refactoring this

    if (passed) {
      this.getParticipants();
      this.course = passed;
      this.loading = false;
      return;
    }
    this.loading = false;
    this.errorLoadPage = true;
  }
  //Get the participants from backend, from specific eventId
  getParticipants() {
    this.loadingParticipants.set(true);
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.participantsService.getParticipantsIndividualEvent_teacher(this.eventId).subscribe({
      next: (res) => {
        if (Array.isArray(res) && !!res) {
          this.loadingParticipants.set(false);
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
      },
    });
  }
  //Button to save the grade and comments of individual student in backend
  btnSaveEdit() {
    const obj = {
      Id: this.editingId()!,
      EventId: this.eventId,
      Grade: this.editParticipants.value.grade.toString(),
      Comments: this.editParticipants.value.comments,
    };

    if (!obj.Grade) {
      alert("You need to insert grade of student")
    }

    //Request to backend
    this.teacherService.insertParticipantGrade(obj).subscribe({
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
    const obj = {
      Id: p.Id,
      EventId: this.eventId,
      EntityId: p.EntityId,
    };

    this.teacherService.updateParticipantStatus(obj).subscribe({
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
  //Button to change status event - Close status
  btnCloseEvent() {
    const participantStatus = this.participants()
      .filter((p) => p.Status == false)
      .map((p) => p.Status);

    const obj = {
      Id: this.eventId,
    };
    console.log(participantStatus.length);

    if (
      participantStatus.length > 0 &&
      this.participants().length > 0 &&
      this.course.Status === 'Ongoing'
    ) {
      this.eventService.updateEventStatusClose(obj).subscribe({
        next: (res) => {
          if (res) {
            this.course.Status = 'Close';
            return;
          }
        },
      });
    } else {
      alert('All students need to be classified and placed inactive');
    }
  }
  //Button to change status event - Ongoing status
  btnOngoingEvent() {
    const participantStatus = this.participants()
      .filter((p) => p.Status == true)
      .map((p) => p.Status);

    const obj = {
      Id: this.eventId,
    };

    if (this.participants().length > 0 && this.course.Status === 'Open') {
      this.eventService.updateEventStatusOngoing(obj).subscribe({
        next: (res) => {
          if (res) {
            this.course.Status = 'Ongoing';
            return;
          }
        },
      });
    } else {
      alert('The course need have student for you can change the status');
    }
  }
}
