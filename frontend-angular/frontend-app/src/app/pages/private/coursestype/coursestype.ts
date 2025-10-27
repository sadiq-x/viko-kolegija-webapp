import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute, ParamMap, RouterLink } from '@angular/router';


type Course = {
  Id: number;
  Name: string;
  Description: string;
  Status: boolean;
  DateCreate: string | Date;
  TopicName?: string;
  Type: string;       // <- usamos para filtrar pelo :type
};

@Component({
  selector: 'app-coursestype',
  imports: [CommonModule, RouterLink],
  templateUrl: './coursestype.html',
  styleUrl: './coursestype.scss',
})
export class CoursesType {
  // parâmetro vindo da rota /courses/type/:type
  type = '';

  // estado de loading
  loading = true;

  // lista já filtrada para o HTML
  courses: Course[] = [];

  // (mock) base de cursos só para demo sem HTTP
  private allCourses = [
    {
      Id: 1,
      Name: 'Angular Fundamentals',
      Description: 'Componentes, formulários reativos e routing.',
      Status: true,
      DateCreate: new Date('2025-08-12'),
      TopicName: 'Tecnologia',
      Type: 'Tecnologia',
    },
    {
      Id: 2,
      Name: 'Treino Funcional',
      Description: 'Força, mobilidade e prevenção de lesões.',
      Status: true,
      DateCreate: new Date('2025-07-01'),
      TopicName: 'Desporto',
      Type: 'Desporto',
    },
    {
      Id: 3,
      Name: 'TypeScript Avançado',
      Description: 'Tipos genéricos, utility types e padrões.',
      Status: false,
      DateCreate: new Date('2025-06-10'),
      TopicName: 'Tecnologia',
      Type: 'Tecnologia',
    },
  ];

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.getCourseFromTopicsPage()
    // reage a alterações do parâmetro :type (navegação dentro da mesma página)
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.type = (params.get('type') ?? '').trim();
      this.loadByType(this.type);
    });
  }

  private loadByType(type: string): void {
    this.loading = true;

    // simula latência de rede só para mostrar o loader do HTML
    setTimeout(() => {
      const norm = (s: string) =>
        s
          .normalize('NFD')
          .replace(/\p{Diacritic}/gu, '')
          .toLowerCase();

      const t = norm(type);

      this.courses = this.allCourses.filter((c) => norm(c.Type) === t);
      this.loading = false;
    }, 300);
  }

  getCourseFromTopicsPage() {
    const passed = history.state?.['course']; // funciona após navegação normal
    if (passed) {
      console.log(passed);

      return;
    }
  }
}
