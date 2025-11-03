import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { ModelListParticipants } from '../../../models/modelParticipant';
import { TeacherService } from '../../../services/teacher';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

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

  participants = signal<ModelListParticipants[]>([]);
  loadingParticipants = signal<boolean>(false);

  editingId = signal<number | null>(null);
  editParticipants: FormGroup;

  eventId: number = 0;

  // TODO: REVER CODIGO
  constructor(
    private route: ActivatedRoute,
    private teacherService: TeacherService,
    private fb: FormBuilder
  ) {
    this.editParticipants = fb.group({
      grade: [
        '',
        [
          Validators.required,
          Validators.min(0),
          Validators.max(20),
          Validators.pattern(/^\d+(\.\d{1,2})?$/), // só números (e decimais opcionais)
        ],
      ],
      comments: ['', [Validators.maxLength(200)]],
    });
  }

  ngOnInit(): void {
    this.getEventFromTeacherPage();
    this.getParticipants();
  }

  //Get the event selected from Page teacher
  getEventFromTeacherPage() {
    const passed = history.state?.['course']; // funciona após navegação normal
    if (passed) {
      //Add the get participants
      this.course = passed;
      this.loading = false;
      return;
    }
  }
  //Get the participants from backend, from specific eventId
  getParticipants() {
    this.loadingParticipants.set(true);
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.teacherService.getParticipantsIndividualEvent(this.eventId).subscribe({
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
  //TODO: To review code, and add update grades and Comments
  //!TESTINGG
  saveEdit(p: any) {
    const obj = {
      Id: this.editingId()!,
      EventId: this.eventId,
      Grade: this.editParticipants.value.grade.toString(),
      Comments: this.editParticipants.value.comments,
    };
    this.teacherService.insertParticipantGrade(obj).subscribe({
      next: (res) => {
        if (res) {
          this.cancelEdit();
          setTimeout(() => {
            this.getParticipants();
          }, 1000);
          console.log(this.editParticipants.value)
          return;
        }
      },
    });
  }
  cancelEdit() {
    this.editingId.set(null);
    this.editParticipants.reset();
  };
  startEdit(p: { Id: number; Grade?: string; Comments?: string }) {
    this.editingId.set(p.Id);
    this.editParticipants.setValue({
      grade: p.Grade ?? '',
      comments: p.Comments ?? '',
    });
    this.editParticipants.markAsPristine();
  }

  closeEvent() {}
}
