import { Component, computed, signal } from '@angular/core';
import { ModelEventsRequest } from '../../../models/modelEvents';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
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

  topics = signal<ModelTopicsResponse[]>([]); //List of all topics
  teachers = signal<ModelTeacherResponse[]>([]); //List of all teachers

  submitting = signal<boolean>(false); //Variable boolean to help in the button to submit

  teacherQuery = signal<string>(''); //Search query Name 

  //Function to filtered all teachers with or without filters
  filteredTeachers = computed<ModelTeacherResponse[]>(() => {
    const q = this.teacherQuery().trim().toLowerCase();
    const list = this.teachers() ?? [];
    if (!q) return list;
    return list.filter((t) => t.Name.toLowerCase().includes(q));
  });

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
      teacherId: [null, [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.getTopics();
    this.getTeachers();
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
          return;
        }
        this.topics.set([]);
      },
    });
  }
  //Get the all teachers from backend
  private getTeachers() {
    this.teacherService.getTeachers().subscribe({
      next: (res) => {
        if (Array.isArray(res) && res) {
          this.teachers.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Name: x.Name ?? x.name,
            }))
          );
          return;
        }
        this.teachers.set([]);
      },
    });
  }
  //Button to create a event
  onCreateCourse() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;

    const obj = {
      Name: v.name,
      Description: v.description,
      TopicsId: v.topicId,
      CreateById: v.teacherId,
    };
    
    this.submitting.set(true);
    this.eventService.createEvent_admin(obj).subscribe({
      next: (res) => {
        if (!res) {
          alert('Could not create course.');
          this.submitting.set(false);
          return;
        }

        alert('Course created successfully.');
        this.form.reset();
        this.teacherQuery.set('')
        this.form.markAsPristine();
        this.form.markAsUntouched();
        this.submitting.set(false);
      },
      error: () => {
        this.submitting.set(false);
        alert('Error creating course.');
      },
    });
  }
  //Helper of button state
  get submitDisabled() {
    return this.form.invalid || this.submitting();
  }
}
