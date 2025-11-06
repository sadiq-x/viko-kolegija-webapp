import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-coursesindividual',
  imports: [CommonModule, RouterLink],
  templateUrl: './coursesindividual.html',
  styleUrl: './coursesindividual.scss',
})
export class CoursesIndividual {
  //Type of topic course
  courseType = {
    Type: ''
  }
  // Estado de loading e erro
  loading = signal(true);
  notFound = signal(false);

  // Dados do evento
  event = signal<any | null>(null);

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.loadCourse()
  }

  loadCourse() {
    console.log(history.state?.['event']);
    const test = history.state?.['event'].TopicName;
    console.log(test)
    this.courseType.Type = test;
    console.log(this.courseType)
    const id = Number(this.route.snapshot.paramMap.get('id'));
    console.log('📌 ID do evento:', id);

    setTimeout(() => {
      const mockEvents = [
        {
          Id: 1,
          Name: 'Frontend Masterclass',
          Description: 'Curso intensivo de Angular e Tailwind CSS.',
          DateCreate: new Date(),
          Status: true,
          TopicName: 'Frontend',
          Results: 'Excelente participação dos alunos.',
        },
      ];

      console.log(this.event());

      this.event.set(mockEvents);

      this.loading.set(false);
    }, 800); // simula 0.8s de atraso
  }
}
