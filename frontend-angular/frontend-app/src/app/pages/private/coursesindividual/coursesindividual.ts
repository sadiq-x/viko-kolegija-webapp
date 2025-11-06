import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

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

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.loadCourse()
  }

  loadCourse() {
   // this.loading.set(true)
    //console.log(history.state?.['event']);
    const test = history.state?.['event'];
    console.log(test)

    if (!test){
      console.log("false")
      this.loading.set(false)
      this.notFound.set(true)
      return;
    }
    this.courseType.Type = test.TopicName;

    console.log(this.courseType.Type)
    const id = Number(this.route.snapshot.paramMap.get('type'));

    const mockEvents = 
        {
          Id: 1,
          Name: 'Frontend Masterclass',
          Description: 'Curso intensivo de Angular e Tailwind CSS.',
          DateCreate: new Date(),
          Status: true,
          TopicName: 'Frontend',
          Results: 'Excelente participação dos alunos.',
        }
      ;
      console.log(mockEvents)


      this.event.set(test);

      this.loading.set(false);
  }
  btnCloseEvent(){}
}
