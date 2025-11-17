import { Component, signal } from '@angular/core';
import { ModelEventsRequest } from '../../../models/modelEvents';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TeacherService } from '../../../services/teacher';
import { EventService } from '../../../services/events';
import { TopicsService } from '../../../services/topics';
import { CommonModule } from '@angular/common';
import { ModelTeacherResponse } from '../../../models/modelTeacher';

@Component({
  selector: 'app-admin-create-events',
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './admin-create-events.html',
  styleUrl: './admin-create-events.scss',
})
export class AdminCreateEvents {
  form!: FormGroup;

  topics = signal<ModelTopicsResponse[]>([]);
  teachers = signal<ModelTeacherResponse[]>([]);

  selectedTeacherName: string | null = null;

  submitting = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private teacherService: TeacherService,
    private eventService: EventService,
    private topicService: TopicsService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      topicId: [null, [Validators.required]],
      entityId: [null, [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.getTopics();
  }
  //Get the all topics from backend
  private getTopics() {
    this.topicService.getTopics().subscribe({
      next: (res) => {
        if (Array.isArray(res) && res) {
          this.topics.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Type: x.Type ?? x.type,
              Description: x.Description ?? x.description,
            }))
          );
        } else {
          this.topics.set([]);
        }
      },
      error: () => this.topics.set([]),
    });
  }
  //Get the all teachers from backend
  private getTeachers() {}
  //Button to create a event
  onCreateCourse() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: ModelEventsRequest = {
      Name: this.form.value.name,
      Description: this.form.value.description,
      TopicsId: this.form.value.topicId,
    };

    this.submitting.set(true);
    this.eventService.createEvent(payload).subscribe({
      next: (res) => {
        if (!res) {
          alert('Could not create course.');
          return;
        } else {
          alert('Course created successfully.');
          this.form.reset();
          this.form.markAsPristine();
          this.form.markAsUntouched();
          this.submitting.set(false);
        }
      },
      error: () => {
        this.submitting.set(false);
        alert('Error creating course.');
        return;
      },
    });
  }
  //Helper of button state
  get submitDisabled() {
    return this.form.invalid || this.submitting();
  }

  onTeacherSelect(name: string) {
    const teacher = this.teachers().find((t) => t.Name === name);
    if (teacher) {
      this.form.patchValue({ topicId: teacher.Id });
    }
  }
}
