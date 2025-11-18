import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { ModelTeacherResponse } from '../../../models/modelTeacher';
import { TopicsService } from '../../../services/topics';
import { TeacherService } from '../../../services/teacher';
import { EventService } from '../../../services/events';

@Component({
  selector: 'app-admin-events',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './admin-events.html',
  styleUrl: './admin-events.scss',
})
export class AdminEvents {
  private eventId: number = 0;
  eventData: any | null = null;

  // formulário de edição
  form: FormGroup;

  topics: ModelTopicsResponse[] = []; //List of all topics
  teachers: ModelTeacherResponse[] = []; //List of all teachers

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private topicService: TopicsService,
    private teacherService: TeacherService,
    private eventService: EventService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      topicName: [null, [Validators.required]],
      teacher: [null, [Validators.required]],
      startDate: [null],
    });
  }

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id')) || 0;
    const coursePassed = history.state?.['course'];

    if (!coursePassed || (coursePassed.Id ?? coursePassed.id) !== this.eventId) {
      return;
    }
    console.log(coursePassed);
    this.getTopics();
    this.getTeachers();

    this.eventData = coursePassed;

    this.patchEventData();
  }

  //Get the all topics from backend
  private getTopics() {
    this.topicService.getTopics().subscribe({
      next: (res) => {
        if (Array.isArray(res) && res) {
          this.topics = res.map((x: any) => ({
            Id: x.Id ?? x.id,
            Type: x.Type ?? x.type,
            Description: x.Description ?? x.description,
          }));
          return;
        }
        this.topics = [];
      },
    });
  }
  //Get the all teachers from backend
  private getTeachers() {
    this.teacherService.getTeachers().subscribe({
      next: (res) => {
        if (Array.isArray(res) && res) {
          this.teachers = res.map((x: any) => ({
            Id: x.Id ?? x.id,
            Name: x.Name ?? x.name,
          }));
          return;
        }
        this.teachers = [];
      },
    });
  }
  //Function to patch event inside the form
  private patchEventData() {
    this.form.patchValue({
      name: this.eventData.Name ?? this.eventData.name,
      description: this.eventData.Description ?? this.eventData.description,
      startDate: this.toDateInputValue(this.eventData.DateCreate ?? this.eventData.dateCreate),
      topicName: this.eventData.TopicName ?? this.eventData.topicName,
      teacher: this.eventData.CreateBy ?? this.eventData.createBy,
    });
  }
  //Function to convert ISO date in a normal date
  private toDateInputValue(value: string | Date | null | undefined): string | null {
    if (!value) return null;
    const d = new Date(value);
    if (Number.isNaN(d.getTime())) return null;
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }
  //Function to verify if the teacher exist in the list of teachers received from backend
  isTeacherInList(teacher: string | null): boolean {
    if (!teacher) return false;
    return this.teachers.some((t) => t.Name === teacher);
  }
  //Button to save edition of event
  btnSave() {
    if (this.form.invalid || !this.eventData) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;

    const payload = {
      Id: this.eventId,
      Name: v.name,
      Description: v.description,
      Type: v.topicName,
      CreateBy: v.teacher,
      DateCreate: new Date(v.startDate).toISOString(),
    };

    this.eventService.updateEvent_admin(payload).subscribe({
      next: (res) => {
        console.log(res);
        if (res) {
          alert('Event update successfully');
        }
      },
    });
  }
  //Button to cancel edition of event
  btnCancel() {
    this.router.navigate(['/admin']);
  }
  //Property to disable the button
  get btnSaveDisabled() {
    return this.form.invalid || !this.form.dirty;
  }
}
